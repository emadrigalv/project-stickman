using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

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
    public bool IsOnLedge {  get; set; }
    public float RotationSpeed => rotationSpeed;

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

        // Handle gravity
        GroundCheck();

        animator.SetBool("isGrounded", isGrounded);// Set animator isGrounded Parameter
        
        if (isGrounded)
        {
            ySpeed = -1.0f;
            velocity = desiredMoveDirection * moveSpeed; // Set velocity when is grounded

            // Limit ledge movement
            IsOnLedge = enviromentScanner.LedgeCheck(desiredMoveDirection, out LedgeData ledgeData);
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
