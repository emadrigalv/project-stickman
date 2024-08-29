using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Rigidbody playerRb;

    [Header("Parameters")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool canJump = true; // SerializeField is not required, only for debugging purposes.

    [Header("Ground Check Sensor")]
    [SerializeField] private float rayCastDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isGrounded; // SerializeField is not required, only for debugging purposes.

    private const float SPEED = 10.0f;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    void Update()
    {
        // Ground check
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, rayCastDistance, whatIsGround);
        
        MyInput();
        SpeedControl();

        // Handle drag
        if (isGrounded) playerRb.drag = groundDrag;
        else playerRb.drag = 0.0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(KeyCode.Space) && canJump && isGrounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (moveDirection.magnitude < 0.1f) return;

        // on ground
        if (isGrounded) 
            playerRb.AddForce(moveDirection.normalized * moveSpeed * SPEED, ForceMode.Force);

        // in air
        else if (!isGrounded)
            playerRb.AddForce(moveDirection.normalized * moveSpeed * SPEED * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 rbVelocitity = new(playerRb.velocity.x, 0.0f, playerRb.velocity.z);

        // limit velocity
        if (rbVelocitity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = rbVelocitity.normalized * moveSpeed;
            playerRb.velocity = new Vector3(limitedVelocity.x, playerRb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        // reset Y velocity
        playerRb.velocity = new Vector3(playerRb.velocity.x, 0.0f, playerRb.velocity.z);

        playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheck.position, Vector3.down * rayCastDistance);
    }
}
