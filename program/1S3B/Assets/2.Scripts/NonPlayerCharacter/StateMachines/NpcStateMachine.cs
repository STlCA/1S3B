using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStateMachine : StateMachine
{
    public Npc _npc { get; }
    public Transform TargetPlayer { get; private set; }

    public NpcIdleState npcIdleState { get; }

    public Vector2 movementInput { get; set; }
    public float movementSpeed { get; private set; }
    public float rotationDamping { get; private set; }
    public float movementSpeedModifier { get; set; } = 1f;

    public NpcStateMachine(Npc npc)
    {
        _npc = npc;

        npcIdleState = new NpcIdleState(this);

        movementSpeed = _npc.npcData.groundedData.baseSpeed;
        rotationDamping = _npc.npcData.groundedData.baseRotationDamping;
    }
}
