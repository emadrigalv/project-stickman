using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using System.Runtime.CompilerServices;

public class ThirdPersonShooterController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;


    private void Update()
    {
        Aiming();
    }

    private void Aiming()
    {
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
        }
    }
}
