using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcIdleState : NpcBaseState
{
    public NpcIdleState(NpcStateMachine npcStateMachine) : base(npcStateMachine)
    {

    }

    public override void Enter()
    {
        _npcSateMachine.movementSpeedModifier = 0f;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
