using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform followTarget;

    [Header("Camera Offsets")]
    [SerializeField] private Vector2 framingOffset;
    [SerializeField] private float distance;

    [Header("Camera Settings")]
    [SerializeField] private float minVerticalAngle;
    [SerializeField] private float maxVerticalAngle;
    [SerializeField] private float verticalSensitivity;
    [SerializeField] private float horizontalSensitivity;
    [SerializeField] private bool invertHorizontalAxis;
    [SerializeField] private bool invertVerticalAxis;

    float rotationY;
    float rotationX;

    float invertVertical;
    float invertHorizontal;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Invert Axis movement if needed
        invertVertical = invertVerticalAxis ? -1 : 1;
        invertHorizontal = invertHorizontalAxis ? -1 : 1;

        // Handle inputs
        rotationX += Input.GetAxis("Mouse Y") * verticalSensitivity * invertVertical;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * horizontalSensitivity * invertHorizontal;

        // Applying camera offsets
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        // Camera movement and rotation
        transform.position = focusPosition -  targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation() => Quaternion.Euler(0, rotationY, 0);    
}
