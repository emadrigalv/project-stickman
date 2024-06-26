using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ParkourController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnviromentScanner scanner;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private ParkourAction ledgeJump;

    [Header("Parameters")]
    [SerializeField] private float autoJumpHeightLimit = 1.5f;
    [SerializeField] private List<ParkourAction> parkourActionList;

    RaycastHit hit;
    ParkourAction theAction;

    private void Update()
    {
        var hitData = scanner.ObstacleCheck();

        if (Input.GetButton("Jump") && !playerController.InAction) // many nested if, refactor needed?
        { 
            hit = hitData.heightHit;
            //Debug.Log("La altura rey " + hit.point);

            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActionList)
                {
                    // Verify what action can be performed depending of the height
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        // Perform parkour action
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }

        if (playerController.IsOnLedge && !playerController.InAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (playerController.LedgeData.height > autoJumpHeightLimit && !Input.GetButton("Jump"))
                shouldJump = false;

            if (shouldJump && playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(ledgeJump));
            }
        }
    }

    private IEnumerator DoParkourAction(ParkourAction action)
    {
        theAction = action;
        playerController.SetControl(false);

        MatchTargetParameters matchParameters = null;
        if (action.EnableTargetMatching)
        {
            matchParameters = new MatchTargetParameters();
            {
                matchParameters.position = action.MatchPos;
                matchParameters.bodyPart = action.MatchBodyPart;
                matchParameters.positionWeight = action.MatchPositionWieght;
                matchParameters.startTime = action.MatchStartTime;
                matchParameters.targetTime = action.MatchTargetTime;
            }
        }

        yield return playerController.DoAction(action.AnimName, matchParameters, action.TargetAnimRotation, 
            action.RotateToObstacle, action.PostActionDelay, action.MirrorAnimation);

        playerController.SetControl(true);
    }

    private void MatchTarget(ParkourAction action)
    {
        //Debug.Log($"Matching target with Position: {action.MatchPos}, Rotation: {transform.rotation}, Matching target: {action.MatchBodyPart}");
        // Change the animation speed to a number below of 1 and this will fix the target matching.

        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPositionWieght, 0), action.MatchStartTime, action.MatchTargetTime);
    }

    private void OnDrawGizmos()
    {
        if (theAction != null)
        {
            // Draw a sphere at the target match position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(theAction.MatchPos, 0.2f);

            // Draw a line from the current player position to the target match position
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, theAction.MatchPos);
        }
    }
}
