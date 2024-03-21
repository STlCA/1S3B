using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NpcMoveState : NpcBaseState
{

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
        if(!IsInChaseRange())
        {
            Debug.Log("근처아님");
        }
        else
        {
            Debug.Log("근처임");
        }

        base.Update();
        // 목적지로 이동
        // 목적지 도착 검사
        // 도착했으면 웨이포인트 타고 다음 목적지 받기
        // destinationWay 방향으로 이동 -> 도착 검사 -> 다음 목적지받기
        Move();
    }

    private void Move()
    {
        Vector3 destinationWay = _npcStateMachine.destinationWay.position - _npcStateMachine._npc.transform.position;

        if (destinationWay.magnitude <= 0.1f)
        {
            Debug.Log("도착");
            // 다음 목적지 받기
            // 웨이포인트의 points가 1증가해야함
            //_npcStateMachine.wayPoint.points++;
        }
        else
        {
            destinationWay.Normalize();
            _npcStateMachine._npc.transform.position += destinationWay * _npcStateMachine.movementSpeedModifier * Time.deltaTime;
        }
    }
   
}
