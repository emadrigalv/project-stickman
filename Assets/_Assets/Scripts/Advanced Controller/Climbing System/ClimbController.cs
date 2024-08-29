using System.Collections;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnviromentScanner scanner;

    private ClimbPoint currentPoint;

    void Update()
    {
        if (!playerController.IsHanging)
        { 
            if (Input.GetButton("Jump") && !playerController.InAction)
            {
                if (scanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
                {
                    currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("Braced Hang", currentPoint.transform, 0.41f, 0.56f));
                }
            }

            if (Input.GetButton("Drop") && !playerController.InAction)
            {
                if (scanner.LedgeDownCheck(out RaycastHit ledgeHit))
                {
                    currentPoint = GetNearestClimbPoint(ledgeHit.transform, ledgeHit.point);

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("Fall Hang", currentPoint.transform, 0.3f, 0.45f, handOffset: new Vector3(0.25f, 0.1f, -0.22f)));
                }
            }
        }
        else
        {
            // Fall Down from Ledge
            if (Input.GetButton("Drop") && !playerController.InAction)
            {
                StartCoroutine(JumpBack());
                return;
            }

            // Ledge to Ledge Jump
            float horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));

            var inputDirection = new Vector2(horizontal, vertical);

            if (playerController.InAction || inputDirection == Vector2.zero) return;

            // Climb to Stand Up from Hanging
            if (currentPoint.MountPoint && inputDirection.y == 1)
            {
                StartCoroutine(HangStandUp());
                return;
            }

            var neightbour = currentPoint.GetNeightbour(inputDirection);

            if (neightbour == null) return;

            if (neightbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentPoint = neightbour.point;

                if (neightbour.direction.y == 1)  // Jump up
                    StartCoroutine(JumpToLedge("Hop Up", currentPoint.transform, 0.35f, 0.66f, handOffset: new Vector3(0.25f, 0.025f, 0.125f)));
                if (neightbour.direction.y == -1) // Jump Down
                    StartCoroutine(JumpToLedge("Hop Down", currentPoint.transform, 0.31f, 0.65f));
                if (neightbour.direction.x == 1) // Jump Right
                    StartCoroutine(JumpToLedge("Hop Right", currentPoint.transform, 0.20f, 0.50f));
                if (neightbour.direction.x == -1) // Jump Left
                    StartCoroutine(JumpToLedge("Hop Left", currentPoint.transform, 0.20f, 0.50f));
            }
            else if (neightbour.connectionType == ConnectionType.Move)
            {
                currentPoint = neightbour.point;

                if (neightbour.direction.x == 1)
                    StartCoroutine(JumpToLedge("Shimmy Right", currentPoint.transform, 0, 0.38f, handOffset: new Vector3(0.25f, 0.01f, 0.1f)));
                else if (neightbour.direction.x == -1)
                    StartCoroutine(JumpToLedge("Shimmy Left", currentPoint.transform, 0, 0.38f, AvatarTarget.LeftHand, handOffset: new Vector3(0.25f, 0.01f, 0.1f)));
            }
        }
    }

    private IEnumerator JumpBack()
    {
        playerController.IsHanging = false;

        yield return StartCoroutine(playerController.DoAction("Jump Back"));

        playerController.ResetTargetRotation();
        playerController.SetControl(true);
    }

    private IEnumerator JumpToLedge(string anim, Transform ledge, float matchStartTime, 
        float matchTargetTime, AvatarTarget hand = AvatarTarget.RightHand, Vector3? handOffset = null)
    {
        var matchParameters = new MatchTargetParameters()
        {
            position = GetHandPosition(ledge, hand, handOffset),
            bodyPart = hand,
            positionWeight = Vector3.one,
            startTime = matchStartTime,
            targetTime = matchTargetTime
        };

        var targetRotation = Quaternion.LookRotation(-ledge.forward);

        yield return playerController.DoAction(anim, matchParameters, targetRotation, true);

        playerController.IsHanging = true;
    }

    private IEnumerator HangStandUp()
    {
        playerController.IsHanging = false;

        yield return StartCoroutine(playerController.DoAction("Hang Stand Up"));

        playerController.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);

        playerController.ResetTargetRotation();
        playerController.SetControl(true);
    }

    private Vector3 GetHandPosition(Transform ledge, AvatarTarget hand, Vector3? handOffset) 
    {
        // This work properly if ledge Z axis is not facing the wall

        var offsetValue = (handOffset != null) ? handOffset.Value : new Vector3(0.25f, 0.035f, 0.075f);

        var handDirection = (hand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;

        return ledge.position + ledge.forward * offsetValue.z + Vector3.up * offsetValue.y - handDirection * offsetValue.x; 
    }

    private ClimbPoint GetNearestClimbPoint(Transform ledge, Vector3 hitPoint) 
    {
        var points = ledge.GetComponentsInChildren<ClimbPoint>();

        ClimbPoint nearestPoint = null;
        float nearestPointDistance = Mathf.Infinity;

        foreach ( var point in points )
        {
            float distance = Vector3.Distance(point.transform.position, hitPoint);

            if ( distance < nearestPointDistance )
            {
                nearestPoint = point;
                nearestPointDistance = distance;
            }
        }

        return nearestPoint;
    }
}
