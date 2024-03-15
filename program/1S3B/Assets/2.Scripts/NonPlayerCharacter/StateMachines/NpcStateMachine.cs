using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateMachine : StateMachine
{
    public Npc _npc { get; }
    public Transform targetPlayer { get; private set; }

    public NpcIdleState npcIdleState { get; }
    public NpcMoveState npcMoveState { get;}

    public Vector2 movementInput { get; set; }
    public float movementSpeed { get; private set; }
    public float rotationDamping { get; private set; }
    public float movementSpeedModifier { get; set; } = 3f;

    public NpcStateMachine(Npc npc)
    {
        _npc = npc;
        targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;

        npcIdleState = new NpcIdleState(this);
        npcMoveState = new NpcMoveState(this);

        movementSpeed = _npc.npcData.groundedData.baseSpeed;
        rotationDamping = _npc.npcData.groundedData.baseRotationDamping;
    }
}