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
            Debug.Log("��ó�ƴ�");
        }
        else
        {
            Debug.Log("��ó��");
        }

        base.Update();
        // �������� �̵�
        // ������ ���� �˻�
        // ���������� ��������Ʈ Ÿ�� ���� ������ �ޱ�
        // destinationWay �������� �̵� -> ���� �˻� -> ���� �������ޱ�
        Move();
    }

    private void Move()
    {
        Vector3 destinationWay = _npcStateMachine.destinationWay.position - _npcStateMachine._npc.transform.position;

        if (destinationWay.magnitude <= 0.1f)
        {
            // ���� ������ �ޱ�
            _npcStateMachine.wayPointIdx++;
            _npcStateMachine.destinationWay = _npcStateMachine.wayPoint.GetPoint(_npcStateMachine.wayPointIdx);
        }
        else
        {
            destinationWay.Normalize();
            _npcStateMachine._npc.transform.position += destinationWay * _npcStateMachine.movementSpeedModifier * Time.deltaTime;
        }
    }
   
}
