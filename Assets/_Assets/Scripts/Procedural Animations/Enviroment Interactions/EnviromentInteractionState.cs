using UnityEngine;

public abstract class EnviromentInteractionState : BaseState<EnviromentInteractionStateMachine.EEnviromentInteractionState>
{
    protected EnviromentInteractionContext Context;

    public EnviromentInteractionState(EnviromentInteractionContext context, 
        EnviromentInteractionStateMachine.EEnviromentInteractionState stateKey) : base(stateKey)
    {
        Context = context;
    }

    private Vector3 GetClosestPointOnCollider(Collider intersectingCollider, Vector3 positionToCheck)
    {
        return intersectingCollider.ClosestPoint(positionToCheck);
    }

    protected void StartIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider.gameObject.layer == LayerMask.NameToLayer("Interactable") && Context.CurrentIntersectingCollider == null) 
        {
            Context.CurrentIntersectingCollider = intersectingCollider;
            Vector3 ClosestPointFromRoot = GetClosestPointOnCollider(intersectingCollider, Context.RootTransform.position);
            Context.SetCurrentSide(ClosestPointFromRoot);

            SetIkTargetPosition();
        }
    }

    protected void UpdateIkTargetPositionTracking(Collider intersectingCollider) 
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider) 
        {
            SetIkTargetPosition();
        }
    }

    protected void ResetIkTargetPositionTracking(Collider intersectingCollider)
    {
        if (intersectingCollider == Context.CurrentIntersectingCollider)
        {
            Context.CurrentIntersectingCollider = null;
            Context.ClosestPointOnColliderFromShoulder = Vector3.positiveInfinity;
        }
    }

    private void SetIkTargetPosition()
    { 
        Context.ClosestPointOnColliderFromShoulder = GetClosestPointOnCollider(Context.CurrentIntersectingCollider, 
        new Vector3(Context.CurrentShoulderTransform.position.x, Context.CharacterShoulderHeight, Context.CurrentShoulderTransform.position.z));

        Vector3 rayDirection = Context.CurrentShoulderTransform.position - Context.ClosestPointOnColliderFromShoulder;
        Vector4 normalizedRayDirection = rayDirection.normalized;

        float offsetDistance = .05f;

        Vector3 offset = normalizedRayDirection * offsetDistance;
        Vector3 offserPosition = Context.ClosestPointOnColliderFromShoulder + offset;

        Context.CurrentIkTargetTransform.position = offserPosition;
    }
}
