using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMoveState : NpcBaseState
{
    //private WayPoint wayPoint;
    private Transform targetWayPoints;
    private int wayPointIndex = 0;

    public NpcMoveState(NpcStateMachine npcStateMachine) : base(npcStateMachine)
    {
    }   

    public override void Enter()
    {
        targetWayPoints = WayPoint.wayPoints[wayPointIndex];
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
            Debug.Log("근처아님");
        }
        else
        {
            Debug.Log("근처임");
        }
        PointsMove();
        
    }

    private void PointsMove()
    {       
        Vector3 direction = targetWayPoints.position - _npcSateMachine._npc.transform.position;
        _npcSateMachine._npc.transform.Translate(direction.normalized * _npcSateMachine.movementSpeedModifier * Time.deltaTime, Space.World); 

        if(Vector3.Distance(_npcSateMachine._npc.transform.position, targetWayPoints.position) <= 0.4f)
        {
            NextWayPoint();
        }
    }

    private void NextWayPoint()
    {
        if(wayPointIndex >= WayPoint.wayPoints.Length - 1)
        {
            wayPointIndex = -1;
            return;
        }
        wayPointIndex++;
        targetWayPoints = WayPoint.wayPoints[wayPointIndex];
    }
}
