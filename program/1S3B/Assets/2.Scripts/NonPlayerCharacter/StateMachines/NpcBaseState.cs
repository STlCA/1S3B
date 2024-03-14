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
        //Patrol();

    }

    /*private void Patrol()
    {
        Vector3 movementDirection = GetMovementDirection();

        Move(movementDirection);
    }

    private void Move(Vector3 direction)
    {
        _npcSateMachine._npc.transform.position += (direction * (5f * 0.225f)) * Time.deltaTime;

        Debug.Log(direction);
    }

    private Vector3 GetMovementDirection()
    {
        Vector3 randomVectorPosition = RandomVector();

        return (randomVectorPosition - _npcSateMachine._npc.transform.position).normalized;
    }

    private Vector3 RandomVector() //¸ñÀûÁö
    {
        Vector3 position = new Vector3(Random.Range(-8.0f, 8.0f), Random.Range(-4.0f, 4.0f), 0);

        return position;
    }

    protected float GetMovementSpeed()
    {
        float movementSpeed = _npcSateMachine.movementSpeed * _npcSateMachine.movementSpeedModifier;
        return movementSpeed;
    }*/

    protected bool IsInChaseRange()
    {
        float playerDistanceSqr = (_npcSateMachine.TargetPlayer.transform.position - _npcSateMachine._npc.transform.position).sqrMagnitude;
        return playerDistanceSqr <= _npcSateMachine._npc.npcData.PlayerChasingRange * _npcSateMachine._npc.npcData.PlayerChasingRange;
    }
}
