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

    [SerializeField] public Transform[] wayPoints;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        //characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();

        npcStateMachine = new NpcStateMachine(this);
    }

    private void Start()
    {
        int randomStartInt = Random.Range(1, 3);
        //int randomStartInt = 2;
        Debug.Log(randomStartInt);
        RandomStart(randomStartInt);
        
    }


    private void Update()
    {
        //npcStateMachine.HandleInput();
        npcStateMachine.Update();
    }

    private void RandomStart(int randomStartInt)
    {
        switch(randomStartInt)
        {
            case 1:
                npcStateMachine.ChangeState(npcStateMachine.npcIdleState);
                Debug.Log("���̵�");
                break;
            case 2:
                npcStateMachine.ChangeState(npcStateMachine.wayPoint);
                break;
            default:
                Debug.Log("����");
                break;
        }
    }

    /*private void FixedUpdate()
    {
        npcStateMachine.Physics2DUpdate();
    }*/
}
