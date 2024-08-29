using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New Parkour Action")]
public class ParkourAction : ScriptableObject
{
    [Header("Parameters")]
    [SerializeField] private string animationName;
    [SerializeField] private string obstacleTag;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private bool rotateToObstacle;
    [SerializeField] private float postActionDelay;

    [Header("Target Matching")]
    [SerializeField] private bool enableTargetMatching = true;
    [SerializeField] private float matchStartTime;
    [SerializeField] private float matchTargetTime;
    [SerializeField] protected AvatarTarget matchBodyPart;
    [SerializeField] private Vector3 matchPositionWieght = new Vector3(0, 1, 0);


    // Expose read values for properties
    public Vector3 MatchPos { get; set; }
    public Quaternion TargetAnimRotation { get; set; }
    public bool MirrorAnimation { get; set; }

    public string AnimName => animationName;
    public bool RotateToObstacle => rotateToObstacle;
    public float PostActionDelay => postActionDelay;

    public bool EnableTargetMatching => enableTargetMatching;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public AvatarTarget MatchBodyPart => matchBodyPart;

    public Vector3 MatchPositionWieght => matchPositionWieght;


    public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        // Check Tag
        if (!string.IsNullOrEmpty(obstacleTag) && hitData.forwardHit.transform.tag != obstacleTag)
            return false;

        // Height Tag
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight || height > maxHeight) return false;

        if (rotateToObstacle) 
            TargetAnimRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPos = hitData.heightHit.point;

        return true;
    }
}

