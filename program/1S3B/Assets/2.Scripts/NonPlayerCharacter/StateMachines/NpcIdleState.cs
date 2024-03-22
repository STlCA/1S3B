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
        _npcStateMachine.movementSpeedModifier = 0f;
        base.Enter();
        StartAnimation(_npcStateMachine._npc.animationData.IdleParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_npcStateMachine._npc.animationData.IdleParameterHash);
    }
}
