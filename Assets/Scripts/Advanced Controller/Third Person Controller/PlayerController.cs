using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private EnviromentScanner enviromentScanner;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 500.0f;

    [Header("Ground Sensor")]
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private Vector3 sensorOffset;
    [SerializeField] private LayerMask groundLayer;

    float forwardAirSpeed;
    float ySpeed;
    bool isGrounded;
    bool hasControl = true;

    Vector3 desiredMoveDirection;
    Vector3 moveDirection;
    Vector3 velocity;

    CameraController camController;
    Quaternion targetRotation;

    public LedgeData LedgeData {  get; set; }
    public float RotationSpeed => rotationSpeed;
    public bool IsOnLedge {  get; set; }
    public bool IsHanging {  get; set; }
    public bool InAction { get; private set; }
    public bool HasControl { get => hasControl; set => hasControl = value; }

    private void Awake()
    {
        camController = Camera.main.GetComponent<CameraController>();

        forwardAirSpeed = moveSpeed / 2.0f;
    }

    private void Update()
    {
        // Initialize velocity
        velocity = Vector3.zero;

        // Handle player Inputs
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        var moveInput = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Input direction function with the camera view direction in the horizontal plane
        desiredMoveDirection = camController.PlanarRotation() * moveInput;
        moveDirection = desiredMoveDirection;

        // Playing animation
        if (!hasControl) return;

        if (IsHanging) return;

        // Handle gravity
        GroundCheck();

        animator.SetBool("isGrounded", isGrounded);// Set animator isGrounded Parameter
        
        if (isGrounded)
        {
            ySpeed = -1.0f;
            velocity = desiredMoveDirection * moveSpeed; // Set velocity when is grounded

            // Limit ledge movement
            IsOnLedge = enviromentScanner.ObstacleLedgeCheck(desiredMoveDirection, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }

            // Set animation
            animator.SetFloat("moveAmount", velocity.magnitude/moveSpeed, 0.2f, Time.deltaTime);
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
            velocity = transform.forward * forwardAirSpeed;  // Set velocity when is in the air
        }
        
        velocity.y = ySpeed; // applying gravity when grounded

        // move player
        characterController.Move(velocity * Time.deltaTime);

        // Handle rotation
        if (moveAmount > 0 && moveDirection.magnitude > 0.2f) targetRotation = Quaternion.LookRotation(moveDirection);
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime); // Rotate player object   
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(sensorOffset), radius, groundLayer);
    }

    void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeData.ledgeWallHit.normal, desiredMoveDirection, Vector3.up);
        float angle = Mathf.Abs(signedAngle);
        
        if(Vector3.Angle(desiredMoveDirection, transform.forward) >= 80)
        {
            // Don't move the player but allow rotation to face the ledge direction
            velocity = Vector3.zero;
            return;
        }

        if(angle < 60)
        {
            velocity = Vector3.zero;
            moveDirection = Vector3.zero;
        }
        else if (angle < 90) // angle b/w 60 and 90, limit velocity to horizontal direction
        {
            var horizontalLedgeDirection = Vector3.Cross(Vector3.up, LedgeData.ledgeWallHit.normal);
            var direction = horizontalLedgeDirection * Mathf.Sign(signedAngle);

            velocity = velocity.magnitude * direction;
            moveDirection = direction;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetParameters matchParameters, Quaternion targetRotation,
        bool rotate = false, float postDelay = 0.0f, bool mirror = false)
    {
        // disable player movement before start animation
        InAction = true;

        animator.SetBool("mirrorAction", mirror);
        animator.CrossFadeInFixedTime(animName, 0.2f); // CrossFadeInFixedTime fix target matching if it is not working
                                                       // due to transition duration betwen animations with big different duration 
        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);

        // Verify if the animation state is the same as the animation that should be performed
        if (!animState.IsName(animName))
            Debug.LogError($"Parkour animation is wrong! {animName}");

        float rotateStartTime =  (matchParameters != null)? matchParameters.startTime : 0.0f;

        float timer = 0.0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;
            float normalizedTimer = timer / animState.length;
            
                // rotate the player towards the obstacle
            if (rotate && normalizedTimer > rotateStartTime)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

            if (matchParameters != null)
            {
                MatchTarget(matchParameters);
            }


            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }

        // Delay action for fragmented animations
        yield return new WaitForSeconds(postDelay);

        InAction = false;
    }

    private void MatchTarget(MatchTargetParameters mp)
    {
        //Debug.Log($"Matching target with Position: {action.MatchPos}, Rotation: {transform.rotation}, Matching target: {action.MatchBodyPart}");
        // Change the animation speed to a number below of 1 and this will fix the target matching.

        if (animator.isMatchingTarget) return;

        animator.MatchTarget(mp.position, transform.rotation, mp.bodyPart,
            new MatchTargetWeightMask(mp.positionWeight, 0), mp.startTime, mp.targetTime);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0.0f);
            targetRotation = transform.rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new(0, 1, 1, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(sensorOffset), radius);
    }
}

public class MatchTargetParameters
{
    public Vector3 position;
    public AvatarTarget bodyPart;
    public Vector3 positionWeight;
    public float startTime;
    public float targetTime;
}
