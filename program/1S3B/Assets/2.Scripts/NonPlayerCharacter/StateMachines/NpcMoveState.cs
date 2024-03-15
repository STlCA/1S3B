using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMoveState : NpcBaseState
{
    //private WayPoint wayPoint;
    private int watPointIndex = 0;

    public NpcMoveState(NpcStateMachine npcStateMachine) : base(npcStateMachine)
    {
    }   

    public override void Enter()
    {
        
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(!IsInChaseRange())
        {
            Debug.Log("��ó�ƴ�");
        }
        else
        {
            Debug.Log("��ó��");
        }
        PointsMove();
        
    }

    private void PointsMove()
    {
        _npcSateMachine._npc.transform.position = Vector2.MoveTowards(_npcSateMachine._npc.transform.position, _npcSateMachine._npc.wayPoints[watPointIndex].transform.position, _npcSateMachine.movementSpeedModifier * Time.deltaTime);

        if (_npcSateMachine._npc.transform.position == _npcSateMachine._npc.wayPoints[watPointIndex].transform.position)
        {
            watPointIndex++;
        }

        if (watPointIndex == _npcSateMachine._npc.wayPoints.Length)
        {
            watPointIndex = 0;
        }
    }
}
