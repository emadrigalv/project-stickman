using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> CurrentState;

    protected bool IsTransitioningState = false;

    private void Start()
    {
        CurrentState.EnterState();
    }

    private void Update()
    {
        EState nextStateKey = CurrentState.GetNextState();

        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
            CurrentState.UpdateState();
        else if (!IsTransitioningState)
            TransitionToState(nextStateKey);
    }

    public void TransitionToState(EState stateKey)
    {
        IsTransitioningState = true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentState.InTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CurrentState.InTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CurrentState.InTriggerExit(other);
    }
}
