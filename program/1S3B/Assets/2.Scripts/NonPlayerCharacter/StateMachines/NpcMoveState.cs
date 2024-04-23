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
        StartAnimation(_npcStateMachine._npc.animationData.WalkParameterHash);
        _npcStateMachine._npc.animator[1].SetBool("IsWalk", true);
        _npcStateMachine._npc.animator[2].SetBool("IsWalk", true);
        _npcStateMachine._npc.animator[3].SetBool("IsWalk", true);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(_npcStateMachine._npc.animationData.WalkParameterHash);
        _npcStateMachine._npc.animator[1].SetBool("IsWalk", false);
        _npcStateMachine._npc.animator[2].SetBool("IsWalk", false);
        _npcStateMachine._npc.animator[3].SetBool("IsWalk", false);
    }

    public override void Update()
    {
        /*if(!IsInChaseRange())
        {
            Debug.Log("��ó�ƴ�");
        }
        else
        {
            Debug.Log("��ó��");
        }*/

        base.Update();
        // �������� �̵�
        // ������ ���� �˻�
        // ���������� ��������Ʈ Ÿ�� ���� ������ �ޱ�
        // destinationWay �������� �̵� -> ���� �˻� -> ���� �������ޱ�
        Move();
        Rotate();
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

    private void Rotate()
    {
        Vector3 destinationWay = _npcStateMachine.destinationWay.position - _npcStateMachine._npc.transform.position;
        float rotateNpc = Mathf.Atan2(destinationWay.y, destinationWay.x) * Mathf.Rad2Deg;
        _npcStateMachine._npc.npcRenderer.flipX = Mathf.Abs(rotateNpc) > 90f;
        _npcStateMachine._npc.npcHairRenderer.flipX = Mathf.Abs(rotateNpc) > 90f;
        _npcStateMachine._npc.npcTopRenderer.flipX = Mathf.Abs(rotateNpc) > 90f;
        _npcStateMachine._npc.npcBottomRenderer.flipX = Mathf.Abs(rotateNpc) > 90f;
    }

}
