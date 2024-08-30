using UnityEngine;

public class TouchState : EnviromentInteractionState
{
    public TouchState(EnviromentInteractionContext context,
        EnviromentInteractionStateMachine.EEnviromentInteractionState estate) : base(context, estate)
    {
        EnviromentInteractionContext Context = context;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override EnviromentInteractionStateMachine.EEnviromentInteractionState GetNextState()
    {
        return StateKey;
    }

    public override void InTriggerEnter(Collider other)
    {

    }

    public override void InTriggerExit(Collider other)
    {

    }

    public override void InTriggerStay(Collider other)
    {

    }
}
