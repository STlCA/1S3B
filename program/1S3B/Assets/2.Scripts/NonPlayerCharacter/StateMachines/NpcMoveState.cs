using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMoveState : NpcBaseState
{

    private WayPointManager wayPointManager;

    //private WayPoint wayPoint;
    

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
        wayPointManager.Update();
        base.Update();
    }

    

}
