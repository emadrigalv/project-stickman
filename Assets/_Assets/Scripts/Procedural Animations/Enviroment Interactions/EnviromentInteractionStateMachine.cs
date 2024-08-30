using UnityEngine.Animations.Rigging;
using UnityEngine;
using UnityEngine.Assertions;

public class EnviromentInteractionStateMachine : StateManager<EnviromentInteractionStateMachine.EEnviromentInteractionState>
{
    public enum EEnviromentInteractionState
    {
        Search,
        Approach,
        Rise,
        Touch,
        Reset,
    }

    [Header("Dependencies")]
    [SerializeField] private TwoBoneIKConstraint _leftIkConstraint;
    [SerializeField] private TwoBoneIKConstraint _rightIkConstraint;
    [SerializeField] private MultiRotationConstraint _leftMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint _rightMultiRotationConstraint;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private CapsuleCollider _rootCollider;

    private EnviromentInteractionContext _context;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(_context != null && _context.ClosestPointOnColliderFromShoulder != null)
        {
            Gizmos.DrawSphere(_context.ClosestPointOnColliderFromShoulder, .03f);
        }
    }

    private void Awake()
    {
        ValidateConstraints();

        _context = new EnviromentInteractionContext(_leftIkConstraint, _rightIkConstraint, _leftMultiRotationConstraint, 
            _rightMultiRotationConstraint, _rigidbody, _rootCollider, transform.root);

        ConstructorEnviromentDetectionCollider();

        InitializeStates();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_leftIkConstraint, "Left IK Constraint is not assigned.");
        Assert.IsNotNull(_rightIkConstraint, "Right IK Constraint is not assigned.");
        Assert.IsNotNull(_leftMultiRotationConstraint, "Left Multi rotation constraint is not assigned.");
        Assert.IsNotNull(_rightMultiRotationConstraint, "Right Multi rotation constraint is not assigned.");
        Assert.IsNotNull(_rigidbody, "Player rigidbody is not assigned.");
        Assert.IsNotNull(_rootCollider, "Root Collider is not assigned.");

    }

    private void InitializeStates()
    {
        // Add States to inherited StateManager "States" dictionary and Set Initial state
        States.Add(EEnviromentInteractionState.Reset, new ResetState(_context, EEnviromentInteractionState.Reset));
        States.Add(EEnviromentInteractionState.Search, new SearchState(_context, EEnviromentInteractionState.Search));
        States.Add(EEnviromentInteractionState.Approach, new ApproachState(_context, EEnviromentInteractionState.Approach));
        States.Add(EEnviromentInteractionState.Rise, new RiseState(_context, EEnviromentInteractionState.Rise));
        States.Add(EEnviromentInteractionState.Touch, new TouchState(_context, EEnviromentInteractionState.Touch));

        CurrentState = States[EEnviromentInteractionState.Reset];
    }

    private void ConstructorEnviromentDetectionCollider()
    {
        float wingspan = _rootCollider.height;

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(wingspan, wingspan, wingspan);
        boxCollider.center = new Vector3(_rootCollider.center.x, _rootCollider.center.y + (.25f * wingspan),
            _rootCollider.center.z + (.5f * wingspan));
        boxCollider.isTrigger = true;
    }
}
