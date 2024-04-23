using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour, ITalk
{
    [field: Header("Animations")]
    [field: SerializeField] public NpcAnimationData animationData { get; private set; }

    [field: Header("References")]
    [field: SerializeField] public NpcSO npcData { get; private set; }

    

    public Rigidbody2D _rigidbody2D { get; private set; }
    public ForceReceiver forceReceiver { get; private set; }
    public WayPointManager wayPointManager { get; private set; }
    public Animator[] animator { get; private set; }
    public SpriteRenderer npcRenderer;
    public SpriteRenderer npcHairRenderer;
    public SpriteRenderer npcTopRenderer;
    public SpriteRenderer npcBottomRenderer;
    //public CharacterController characterController { get; private set; }

    private NpcStateMachine npcStateMachine;

    GameManager gameManager;
    DataManager dataManager;  
    NpcDataBese npcDataBese;
    TalksDatabese talksDatabese;

    NpcInfo npcInfo;
    TalkInfo talkInfo;

    public int npcId;

    //[SerializeField] public Transform[] wayPoints;

    private void Awake()
    {
        animator = new Animator[3];
        animationData.Initialize();
        animator = GetComponentsInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        //characterController = GetComponent<CharacterController>();
        forceReceiver = GetComponent<ForceReceiver>();
        
    }

    private void Start()
    {
        wayPointManager = WayPointManager.instance;
        npcStateMachine = new NpcStateMachine(this, wayPointManager);

        RandomStartState();
        gameManager = GameManager.Instance;
        dataManager = gameManager.DataManager;

        npcDataBese = dataManager.npcDatabese;
        talksDatabese = dataManager.talksDatabese;

        npcInfo = npcDataBese.GetNpcByKey(npcId);
        talkInfo = talksDatabese.GetTalkByKey(npcInfo.talk_Id);
    }

    private void RandomStartState()
    {
        //int randomStartInt = Random.Range(1, 3);
        int randomStartInt = 2;
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
                Debug.Log("Idle");
                break;
            case 2:
                npcStateMachine.ChangeState(npcStateMachine.npcMoveState);
                Debug.Log("Move");
                break;
            default:
                Debug.Log("Exception");
                break;
        }
    }

    public void Talk()
    {
        // 대화 진행
        Debug.Log("대화 시도");

        if(talkInfo.npcDialogue == null)
        {
            gameManager.talkPanel.SetActive(false);
        }
        else
        {
            gameManager.talkPanel.SetActive(true);
            StartCoroutine("PrintDialogue");
            npcStateMachine.movementSpeedModifier = 0f;
        }
    }

    IEnumerator PrintDialogue()
    {
        for (int i = 0; i < talkInfo.npcDialogue.Count; i++)
        {
            Debug.Log(talkInfo.npcDialogue[i]);
            gameManager.npcNameText.text = npcInfo.npcName;
            gameManager.talkText.text = talkInfo.npcDialogue[i];
            yield return new WaitForSeconds(3f);
        }
    }

    public void CloseTalkPanel()
    {
        gameManager.talkPanel.SetActive(false);
        npcStateMachine.movementSpeedModifier = 1f;
    }
}
