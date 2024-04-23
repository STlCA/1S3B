using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBaseState : IState
{
    protected NpcStateMachine _npcStateMachine;

    protected readonly NpcGroundData npcGroundData;


    public NpcBaseState(NpcStateMachine npcStateMachine)
    {
        _npcStateMachine = npcStateMachine;
        npcGroundData = _npcStateMachine._npc.npcData.groundedData;
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
        float playerDistanceSqr = (_npcStateMachine.targetPlayer.transform.position - _npcStateMachine._npc.transform.position).sqrMagnitude;
        return playerDistanceSqr <= _npcStateMachine._npc.npcData.PlayerChasingRange * _npcStateMachine._npc.npcData.PlayerChasingRange;
    }

    protected void StartAnimation(int animationHash)
    {
        _npcStateMachine._npc.animator[0].SetBool(animationHash, true);
       
    }

    protected void StopAnimation(int animationHash)
    {
        _npcStateMachine._npc.animator[0].SetBool(animationHash, false);
        
    }
}
