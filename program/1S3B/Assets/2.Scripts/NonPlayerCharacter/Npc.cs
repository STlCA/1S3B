using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [field: Header("References")]
    [field: SerializeField] public NpcSO npcData { get; private set; }

    public Rigidbody2D rigidbody2D { get; private set; }
    public ForceReceiver forceReceiver { get; private set; }
    //public CharacterController characterController { get; private set; }

    private NpcStateMachine npcStateMachine;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        //characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();

        npcStateMachine = new NpcStateMachine(this);
    }

    private void Start()
    {
        npcStateMachine.ChangeState(npcStateMachine.npcIdleState);
    }


    private void Update()
    {
        npcStateMachine.HandleInput();
        npcStateMachine.Update();
    }

    private void FixedUpdate()
    {
        npcStateMachine.Physics2DUpdate();
    }
}
