using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnviromentInteractionContext 
{
    public enum EBodySide
    {
        RIGHT,
        LEFT
    }

    private TwoBoneIKConstraint _leftIkConstraint;
    private TwoBoneIKConstraint _rightIkConstraint;
    private MultiRotationConstraint _leftMultiRotationConstraint;
    private MultiRotationConstraint _rightMultiRotationConstraint;
    private Rigidbody _rigidbody;
    private CapsuleCollider _rootCollider;
    private Transform _rootTransform;

    // Constructor
    public EnviromentInteractionContext(TwoBoneIKConstraint leftIkConstraint, TwoBoneIKConstraint rightIkConstraint, 
        MultiRotationConstraint leftMultiRotationConstraint, MultiRotationConstraint rightMultiRotationConstraint, 
        Rigidbody rigidbody, CapsuleCollider rootCollider, Transform rootTransform)
    {
        _leftIkConstraint = leftIkConstraint;
        _rightIkConstraint = rightIkConstraint;
        _leftMultiRotationConstraint = leftMultiRotationConstraint;
        _rightMultiRotationConstraint = rightMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
        _rootTransform = rootTransform;

        CharacterShoulderHeight = leftIkConstraint.data.root.transform.position.y;
    }

    //Read-only properties
    public TwoBoneIKConstraint LeftIkConstraint => _leftIkConstraint;
    public TwoBoneIKConstraint RightIkConstraint => _rightIkConstraint;
    public MultiRotationConstraint LeftMultiRotationConstraint => _leftMultiRotationConstraint;
    public MultiRotationConstraint RotationConstraint => _rightMultiRotationConstraint;
    public Rigidbody Rigidbody => _rigidbody;
    public CapsuleCollider RootCollider => _rootCollider;
    public Transform RootTransform => _rootTransform;

    public float CharacterShoulderHeight { get; private set; }

    public Collider CurrentIntersectingCollider { get; set; }
    public TwoBoneIKConstraint CurrentIkConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }
    public Vector3 ClosestPointOnColliderFromShoulder {  get; set; } = Vector3.positiveInfinity;

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftIkConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightIkConstraint.data.root.transform.position;

        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) < Vector3.Distance(positionToCheck, rightShoulder);

        if ((isLeftCloser))
        {
            CurrentBodySide = EBodySide.LEFT;
            CurrentIkConstraint = _leftIkConstraint;
            CurrentMultiRotationConstraint = _leftMultiRotationConstraint;
        }
        else
        {
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIkConstraint = _rightIkConstraint;
            CurrentMultiRotationConstraint = _rightMultiRotationConstraint;
        }

        CurrentShoulderTransform = CurrentIkConstraint.data.root.transform;
        CurrentIkTargetTransform = CurrentIkConstraint.data.target.transform;
    }
}
