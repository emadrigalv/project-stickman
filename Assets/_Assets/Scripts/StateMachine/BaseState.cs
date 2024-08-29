using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState<EState> where EState : Enum
{
    public EState StateKey { get; private set; }

    public BaseState(EState key)
    {
        StateKey = key;
    }

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    public abstract EState GetNextState();
    public abstract void InTriggerEnter(Collider other);
    public abstract void InTriggerStay(Collider other);
    public abstract void InTriggerExit(Collider other);
}
