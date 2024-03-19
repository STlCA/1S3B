using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBaseState : IState
{
    protected NpcStateMachine _npcSateMachine;

    protected readonly NpcGroundData npcGroundData;


    public NpcBaseState(NpcStateMachine npcStateMachine)
    {
        _npcSateMachine = npcStateMachine;
        npcGroundData = _npcSateMachine._npc.npcData.groundedData;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {
      

    }

    protected bool IsInChaseRange()
    {
        float playerDistanceSqr = (_npcSateMachine.targetPlayer.transform.position - _npcSateMachine._npc.transform.position).sqrMagnitude;
        return playerDistanceSqr <= _npcSateMachine._npc.npcData.PlayerChasingRange * _npcSateMachine._npc.npcData.PlayerChasingRange;
    }
}
