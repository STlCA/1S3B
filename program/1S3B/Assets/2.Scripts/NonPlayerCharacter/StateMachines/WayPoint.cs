using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : NpcBaseState
{
    

    private int watPointIndex = 0;
    

    public WayPoint(NpcStateMachine npcStateMachine) : base(npcStateMachine)
    {
    }

    private void Awake()
    {
        
    }

    public override void Enter()
    {
        _npcSateMachine._npc.transform.position = _npcSateMachine._npc.wayPoints[watPointIndex].transform.position;
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        PointsMove();
    }

    private void PointsMove()
    {
        _npcSateMachine._npc.transform.position = Vector2.MoveTowards(_npcSateMachine._npc.transform.position, _npcSateMachine._npc.wayPoints[watPointIndex].transform.position, _npcSateMachine.movementSpeedModifier * Time.deltaTime);

        if(_npcSateMachine._npc.transform.position == _npcSateMachine._npc.wayPoints[watPointIndex].transform.position)
        {
            watPointIndex++;
        }

        if(watPointIndex == _npcSateMachine._npc.wayPoints.Length)
        {
            watPointIndex = 0;
        }
    }
}
