using UnityEngine;
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
        var velocity = Vector3.zero;

        // Handle player Inputs
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));

        var moveInput = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // Input direction function with the camera view direction in the horizontal plane
        var moveDirection = camController.PlanarRotation() * moveInput;

        // Playing animation
        if (!hasControl) return;

        // Handle gravity
        GroundCheck();

        animator.SetBool("isGrounded", isGrounded);// Set animator isGrounded Parameter
        
        if (isGrounded)
        {
            ySpeed = -1.0f;
            velocity = moveDirection * moveSpeed; // Set velocity when is grounded

            IsOnLedge = enviromentScanner.LedgeCheck(moveDirection, out LedgeData ledgeData);
            LedgeData = ledgeData;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
            velocity = transform.forward * forwardAirSpeed;  // Set velocity when is in the air
        }
        
        velocity.y = ySpeed; // applying gravity when grounded

        // move player
        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0) targetRotation = Quaternion.LookRotation(moveDirection);
        
        // Rotate player object
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        // Set animation
        animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);    
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(sensorOffset), radius, groundLayer);
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
