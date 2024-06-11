using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnviromentScanner scanner;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    [Header("Parameters")]
    [SerializeField] private List<ParkourAction> parkourActionList;


    bool inAction;
    public bool isMatching = false;
    RaycastHit hit;
    ParkourAction theAction;

    private void Update()
    {
        if (Input.GetButton("Jump") && !inAction) // many nested if, refactor needed?
        { 
            var hitData = scanner.ObstacleCheck();
            
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
    }

    private IEnumerator DoParkourAction(ParkourAction action)
    {
        // disable player movement before start animation
        inAction = true;
        playerController.SetControl(false);

        animator.SetBool("mirrorAction", action.MirrorAnimation);
        animator.CrossFade(action.AnimName, 0.2f);

        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);

        // Verify if the animation state is the same as the animation that should be performed
        if (!animState.IsName(action.AnimName))
            Debug.LogError($"Parkour animation is wrong! {action.AnimName}");

        float timer = 0.0f;

        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            //Debug.Log("Inverse normal of the obstacle face: " + hit.normal);

            // rotate the player towards the obstacle
            if (action.RotateToObstacle)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetAnimRotation, playerController.RotationSpeed * Time.deltaTime);

            if (action.EnableTargetMatching)
            {
                animator.applyRootMotion = true;
                MatchTarget(action);
            }
            

            if (animator.IsInTransition(0) && timer > 0.5f)
                break;

            yield return null;
        }

        isMatching = false;

        // Delay action for fragmented animations
        yield return new WaitForSeconds(action.PostActionDelay);

        playerController.SetControl(true);
        inAction = false;
    }

    private void MatchTarget(ParkourAction action)
    {
        //Debug.Log($"Matching target with Position: {action.MatchPos}, Rotation: {transform.rotation}, Matching target: {animator.isMatchingTarget}");
        // Change the animation speed to a number below of 1 and this will fix the target matching.

        if (animator.isMatchingTarget) return;

        theAction = action;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart,
            new MatchTargetWeightMask(action.MatchPositionWieght, 0), action.MatchStartTime, action.MatchTargetTime);

        isMatching = true;
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
