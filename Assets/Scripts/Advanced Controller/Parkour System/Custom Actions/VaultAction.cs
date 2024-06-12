using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New Vault Action")]
public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        if (!base.CheckIfPossible(hitData, player)) return false;

        var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

        if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
        {
            // Mirror the animation
            MirrorAnimation = true;
            matchBodyPart = AvatarTarget.RightHand;
        }
        else
        {
            // Don't Mirror
            MirrorAnimation = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }

        return true;
    }
}
