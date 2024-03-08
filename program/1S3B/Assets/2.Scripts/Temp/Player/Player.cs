using System;
using UnityEngine;
using UnityEngine.Tilemaps;

enum TurnInfor
{
    Back,
    Front,
    Left,
    Right
}

public class Player : MonoBehaviour
{
    // ���� �ν�����
    Animator animator;
    AudioSource audioSource;
    public PlayerEvent playerEvent;

    // �ڽ� ������Ʈ
    public Animator coinAni;
    public Animator installAni;

    // �ܺ� ������Ʈ
    GameManager gameManager;
    public Tilemap groundTile;
    public Tilemap obstacleTile;
    public Tilemap previewTile;

    // ���� ��
    public float speed;
    public float maxHealth;
    public float maxFeed;

    // ���� ���� ����
    bool isIdle;
    bool isAction;
    public int turnNum; // 0 : ��, 1 : ��, 2 : ��, 3 : ��
    public float nowHealth;
    public float nowFeed;
    float flowFeedTime;
    Vector3 turnDir = Vector2.down;
    public Vector3Int frontPos;
    RaycastHit2D objHit;
    RaycastHit2D groundHit;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        coinAni = transform.GetChild(0).GetComponent<Animator>();
        installAni = transform.GetChild(1).GetComponent<Animator>();
    }

    public void SetInit()
    {
        // �������� ����
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerEvent = GetComponent<PlayerEvent>();

        groundTile = GameObject.Find("Grid").transform.Find("Ground").GetComponent<Tilemap>();
        obstacleTile = GameObject.Find("Grid").transform.Find("Obstacle").GetComponent<Tilemap>();
        previewTile = GameObject.Find("Grid").transform.Find("Preview").GetComponent<Tilemap>();

        gameManager.QuickSlotEvent(0);

        gameManager.SetHealth(maxHealth);
        gameManager.SetFeed(maxFeed);

        frontPos = new Vector3Int(
            Mathf.FloorToInt(transform.position.x),
            Mathf.FloorToInt(transform.position.y + 1)
        );
    }

    void Update()
    {
        if (gameManager.isStart)
        {
            SetAniState();
            InputEvent();
            CheckObject();
            DownFeed();
            Move();
        }
    }

    void Move()
    {
        if (InhibitAction()) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        bool isMoveH = h > 0 || h < 0;
        bool isMoveV = v > 0 || v < 0;

        if (isMoveH || isMoveV)
        {
            // �̸����� ���
            playerEvent.PreviewInstallBlock(gameManager.nowQuickItem, GetNowFrontPos(), false);

            transform.position += new Vector3(h, v) * speed * Time.deltaTime;
            isIdle = false;
        }
        else isIdle = true;

        switch ((int)h)
        {
            case -1:
                turnNum = 2; // ��
                turnDir = Vector2.left;
                break;
            case 1:
                turnNum = 3; // ��
                turnDir = Vector2.right;
                break;
        }
        switch ((int)v)
        {
            case -1:
                turnNum = 0; // ��
                turnDir = Vector2.down;
                break;
            case 1:
                turnNum = 1; // ��
                turnDir = Vector2.up;
                break;
        }

        animator.SetInteger("h", (int)h);
        animator.SetInteger("v", (int)v);
        animator.SetInteger("turnNum", turnNum);
        animator.SetBool("isIdle", isIdle);
    }

    void DownFeed()
    {
        flowFeedTime += Time.deltaTime;

        if (flowFeedTime > Setting.PLAYER_FEEDINTERVAL)
        {
            if (nowFeed > 0)
                gameManager.SetFeed(-0.5f);
            else
                gameManager.SetFeed(-0.5f);
            flowFeedTime = 0;
        }
    }

    void SetAniState()
    {
        if (GameManager.IsGetAniState(animator, $"player_hoe{Enum.GetName(typeof(TurnInfor), turnNum)}", 1.0f))
            SetAnimation("isHoe", false);
        if (GameManager.IsGetAniState(animator, $"player_axe{Enum.GetName(typeof(TurnInfor), turnNum)}", 1.0f))
            SetAnimation("isAxe", false);
        if (GameManager.IsGetAniState(animator, $"player_water{Enum.GetName(typeof(TurnInfor), turnNum)}", 1.0f))
            SetAnimation("isWater", false);
    }

    void InputEvent()
    {
        // 49 ~ 56 = 1 ~ 9���� �����е�
        if (!string.IsNullOrEmpty(Input.inputString) && (int)Input.inputString[0] >= 49 && (int)Input.inputString[0] <= 56)
        {
            gameManager.nowQuickIndex = (int)Input.inputString[0] - 49;
            gameManager.QuickSlotEvent(gameManager.nowQuickIndex);
        }

        if (Input.GetKeyDown(KeyCode.E) && !gameManager.panel_chest.activeSelf)
        {
            GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["openInventory"], false);
            OpenInventory(Vector2.zero);
        }

        if(Input.GetKeyDown(KeyCode.Space) && gameManager.panel_npcDialogue.activeSelf)
        {
            bool isEndDialogue = playerEvent.StopDialogue();
            if(!isEndDialogue) StartCoroutine(playerEvent.DialogueEvent(0));
        }
    }

    void CheckObject()
    {
        // ������Ʈ
        int expectGroundLayer = ~((1 << LayerMask.NameToLayer("Ground")) | (1 << LayerMask.NameToLayer("Building"))) | (1 << LayerMask.NameToLayer("Obstacle"));
        objHit = Physics2D.Raycast(transform.position + (turnDir / 2), turnDir, 1f, expectGroundLayer);
        if (!InhibitAction() && Input.GetKeyDown(KeyCode.Space) && objHit)
        {
            ObjectEvent(objHit.collider);
        }

        groundHit = Physics2D.Raycast(transform.position + (turnDir / 2), turnDir / 2, 1f, 1 << LayerMask.NameToLayer("Ground")); 
        if(!InhibitAction() && Input.GetKeyDown(KeyCode.Space) && groundHit)
        {
            GroundEvent(objHit.collider);
        }
    }

    void ObjectEvent(Collider2D hit)
    {
        // ����
        if (hit.CompareTag("Chest"))
        {
            GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["chest"], false);
            playerEvent.OpenChest(hit.GetComponent<Chest>());
        }
        if (hit.CompareTag("Shop"))
        {
            GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["shop"], false);
            playerEvent.OpenShop(hit.GetComponent<Shop>());
        }
        if (hit.CompareTag("Npc"))
        {
            playerEvent.OpenNpcDialogue(in hit);
        }

        ItemImg nowQuickItem = gameManager.nowQuickItem;

        if (nowQuickItem != null)
        {
            // ����
            if(hit.CompareTag("Tree") && nowQuickItem.CompareTag("Axe"))
            {
                GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["tool"], false);
                GameManager.PlaySound(gameManager.audioSource, gameManager.resourcesManager.sounds["hitTree"], false);

                playerEvent.CutTree(nowQuickItem, hit.GetComponent<Tree>());
            }
        }
    }

    void GroundEvent(Collider2D objHit)
    {
        ItemImg nowQuickItem = gameManager.nowQuickItem;

        if (nowQuickItem != null)
        {
            // �� ��ġ (�ܵ��϶��� ��ġ ����)
            if (nowQuickItem.CompareTag("Item_Ground") && groundTile.GetTile(frontPos).name.Equals("Grass_Basic"))
            {
                GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["install"], false);
                playerEvent.InstallSoil(nowQuickItem);
            }

            // ��Ÿ�� ��ġ
            if (nowQuickItem.CompareTag("Fence") && objHit == null)
            {
                GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["install"], false);
                playerEvent.InstallFence(nowQuickItem);
            }

            // ���� ��ġ & ���� ���� �ֱ� (���� ���̸鼭 ������ ������ ��ġ)
            if (nowQuickItem.CompareTag("Seed"))
            {
                if (objHit == null && groundTile.GetTile(frontPos).name.Equals("Soil_Water"))
                {
                    GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["seed"], false);
                    playerEvent.InstallSeed(nowQuickItem);
                }

                if (objHit != null && objHit.CompareTag("Animal"))
                {
                    GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["eat_animal"], false);
                    playerEvent.FeedAnimal(nowQuickItem, objHit.GetComponent<Animal>());
                }
            }

            // ȣ�̷� ��ġ�ߴ� �� ȸ�� (�ܵ� �ƴ� ��쿡�� ȸ�� ����)
            if (nowQuickItem.CompareTag("Hoe"))
            {
                if (!groundTile.GetTile(frontPos).name.Equals("Grass_Basic")){
                    GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["tool"], false);
                    playerEvent.WithDrawTile(nowQuickItem, groundTile, $"{groundTile.GetTile(frontPos).name}PrefebImg");
                }
            }

            // ������ ��Ÿ�� �ı�
            if (nowQuickItem.CompareTag("Axe"))
            {
                if (obstacleTile.GetTile(frontPos) != null && obstacleTile.GetTile(frontPos).name.Contains("Fence"))
                {
                    GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["tool"], false);
                    playerEvent.WithDrawTile(nowQuickItem, obstacleTile, $"Fence_BasicPrefebImg");
                }
            }

            // �� �Ѹ��� ä��� �̺�Ʈ (���� ���̸鼭 ������ ������)
            if (nowQuickItem.CompareTag("WateringCan") && objHit == null)
            {
                // ���� ������� ��
                if (nowQuickItem.name.Contains("Fill") && groundTile.GetTile(frontPos).name.Equals("Soil_Basic"))
                {
                    GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["water"], false);
                    playerEvent.PourWatering(nowQuickItem);
                }
                // ���� ���� ��
                else if (groundTile.GetTile(frontPos).name.Equals("Water_Basic"))
                {
                    GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["water"], false);
                    playerEvent.FillWatering(nowQuickItem);
                }
            }

            if(nowQuickItem.CompareTag("Bottle") && objHit != null && objHit.name.Equals(GameManager.GetPrefebName("Cow")))
            {
                GameManager.PlaySound(audioSource, gameManager.resourcesManager.sounds["water"], false);
                playerEvent.GetMilk(nowQuickItem);
            }

            // ���� ��ȯ
            if (nowQuickItem.CompareTag("Animal"))
            {
                playerEvent.SpawnAniaml(nowQuickItem);
            }
        }
    }

    public void OpenInventory(Vector2 pos)
    {
        gameManager.panel_inventory.SetActive(true);
        gameManager.panel_inventory.GetComponent<RectTransform>().anchoredPosition = pos;
        gameManager.inventory.ChangeCategory(0);
    }

    public void ShowAniEvent(Animator ani)
    {
        ani.gameObject.SetActive(true);
        ani.SetBool("isShow", true);
    }

    public void SetAnimation(string aniName, bool state)
    {
        animator.SetBool(aniName, state);
        isAction = state;
    }

    public Vector3Int GetNowFrontPos()
    {
        Vector3Int newFrontPos = new Vector3Int(
            Mathf.FloorToInt(transform.position.x + turnDir.x),
            Mathf.FloorToInt(transform.position.y + turnDir.y)
        );

        return newFrontPos;
    }

    public bool ProhibitPickUp()
    {
        return gameManager.quickSlot.items.Count == gameManager.quickSlot.panel_items.Length;
    }

    bool InhibitAction()
    {
        return gameManager.panel_npcDialogue.activeSelf || gameManager.panel_inventory.activeSelf || isAction;
    }

    public bool CanShowInstallPreview()
    {
        return gameManager.nowQuickItem.CompareTag("Item_Ground") || gameManager.nowQuickItem.CompareTag("Fence");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(playerEvent != null) playerEvent.PickupEvent(collision);
    }
}
