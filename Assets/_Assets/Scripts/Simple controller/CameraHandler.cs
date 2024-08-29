using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ThirdPersonCam;

public class CameraHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject explorationCamera;
    [SerializeField] private GameObject combatCamera;

    public CameraStyle currentStyle;

    public enum CameraStyle
    {
        Exploration,
        Combat
        //Topdown
    }

    private void Start()
    {
        currentStyle = CameraStyle.Exploration;
        SwitchCameraStyle(currentStyle);
    }

    private void Update()
    {
        // switch camera style
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCameraStyle(CameraStyle.Exploration);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCameraStyle(CameraStyle.Combat);
    }

    private void SwitchCameraStyle(CameraStyle newStyle)
    {
        combatCamera.SetActive(false);
        explorationCamera.SetActive(false);

        if (newStyle == CameraStyle.Exploration) explorationCamera.SetActive(true);
        if (newStyle == CameraStyle.Combat) combatCamera.SetActive(true);

        currentStyle = newStyle;
    }
}
