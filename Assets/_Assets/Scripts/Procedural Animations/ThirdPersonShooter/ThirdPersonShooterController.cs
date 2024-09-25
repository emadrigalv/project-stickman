using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private GameObject debugprefab;
    [SerializeField] private Animator animator;

    [Header("Parameters")]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderMask;
    [SerializeField] private Transform debugTransform;

    Vector3 mouseWorldPosition;


    private void Update()
    {
        // Read center screen and mouse position

        mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new(Screen.width / 2f, Screen.height / 2f);
        
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        Aiming();

        if (starterAssetsInputs.shoot)
        {
            if (hitTransform != null) 
            {
                Instantiate(debugprefab, debugTransform.position, Quaternion.identity);
            }

            starterAssetsInputs.shoot = false;
        }
    }

    private void Aiming()
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }
    }
}
