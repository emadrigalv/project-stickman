using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnviromentScanner scanner;


    void Update()
    {
        if (!playerController.IsHanging)
        { 
            if (Input.GetButton("Jump") && !playerController.InAction)
            {
                if (scanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("Braced Hang", ledgeHit.transform, 0.41f, 0.56f));
                }
            }
        }
        else
        {
            // Implement Ledge to Ledge Jump
        }
    }

    private IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, float matchTargetTime)
    {
        var matchParameters = new MatchTargetParameters()
        {
            position = GetHandPosition(ledge),
            bodyPart = AvatarTarget.RightHand,
            positionWeight = Vector3.one,
            startTime = matchStartTime,
            targetTime = matchTargetTime
        };

        var targetRotation = Quaternion.LookRotation(-ledge.forward);

        yield return playerController.DoAction(anim, matchParameters, targetRotation, true);

        playerController.IsHanging = true;
    }

    private Vector3 GetHandPosition(Transform ledge)
    {
        // This work properly if ledge Z axis is not facing the wall
        return ledge.position + ledge.forward * 0.05f + Vector3.up * 0.025f - ledge.right * 0.25f; 
    }
}
