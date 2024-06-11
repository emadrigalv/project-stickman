using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform combatLookAt;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private CameraHandler cameraHandler;

    [Header("Parameters")]
    [SerializeField] private float rotationSpeed;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //rotate orientation
        Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDirection.normalized;

        //rotate player object
        if (cameraHandler.currentStyle == CameraHandler.CameraStyle.Exploration)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDirection.magnitude > 0.1f)
            {
                Vector3 targetForward = Vector3.Slerp(playerVisual.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
                targetForward.y = 0.0f; // lock the y-axis to avoid tilting

                playerVisual.forward = targetForward;
            }
            // lock the x and z rotations
            else
                playerVisual.rotation = Quaternion.Euler(0, playerVisual.rotation.eulerAngles.y, 0);
        }
        
        else if (cameraHandler.currentStyle == CameraHandler.CameraStyle.Combat) // like FPS?
        {
            Vector3 combatViewDirection = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);
            orientation.forward = combatViewDirection.normalized;

            playerVisual.forward = combatViewDirection.normalized;
        }
    }
}
