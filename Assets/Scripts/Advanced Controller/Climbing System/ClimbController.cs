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
                    currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();

                    playerController.SetControl(false);
                    StartCoroutine(JumpToLedge("Braced Hang", ledgeHit.transform, 0.41f, 0.56f));
                }
            }
        }
        else
        {
            // Ledge to Ledge Jump
            float horizontal = Mathf.Round(Input.GetAxisRaw("Horizontal"));
            float vertical = Mathf.Round(Input.GetAxisRaw("Vertical"));

            var inputDirection = new Vector2(horizontal, vertical);

            if (playerController.InAction || inputDirection == Vector2.zero) return;

            var neightbour = currentPoint.GetNeightbour(inputDirection);

            if (neightbour == null) return;

            if (neightbour.connectionType == ConnectionType.Jump && Input.GetButton("Jump"))
            {
                currentPoint = neightbour.point;

                if (neightbour.direction.y == 1)  // Jump up
                    StartCoroutine(JumpToLedge("Hop Up", currentPoint.transform, 0.35f, 0.66f));
                if (neightbour.direction.y == -1) // Jump Down
                    StartCoroutine(JumpToLedge("Hop Down", currentPoint.transform, 0.31f, 0.65f));
                if (neightbour.direction.x == 1) // Jump Right
                    StartCoroutine(JumpToLedge("Hop Right", currentPoint.transform, 0.20f, 0.50f));
                if (neightbour.direction.x == -1) // Jump Left
                    StartCoroutine(JumpToLedge("Hop Left", currentPoint.transform, 0.20f, 0.50f));

            }
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
