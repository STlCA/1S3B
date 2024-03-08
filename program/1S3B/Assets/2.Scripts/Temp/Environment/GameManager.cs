
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ���� �ν�����
    public AudioSource audioSource;

    // �ܺ� ������Ʈ
    public Transform canvas;
    public ResourcesManager resourcesManager;
    public ObjectManager objectManager;
    public Player player;
    public ItemEvent itemEvent;
    public Transform seedsParent;

    // UI
    public GameObject panel_loading;
    public GameObject panel_inventory;
    public GameObject panel_chest;
    public GameObject panel_quickSlots;
    public GameObject panel_shop;
    public GameObject panel_npcDialogue;
    public Scrollbar healthbar;
    public Scrollbar feedbar;

    // ������ �ν��Ͻ�
    public ItemListParent myItem;
    public QuickSlot quickSlot;
    public Inventory inventory;
    public PlayerResource playerResource;
    public List<Chest> chests = new List<Chest>();
    public List<Quest> nowQuest = new List<Quest>();

    // ���� ���� ����
    public bool isStart;
    public int nowQuickIndex;
    public Chest nowOpenChest;
    public Image img_nowQuickSlot;
    public ItemImg nowQuickItem;
    public List<Stack<Animal>> canMeetingAnimal = new List<Stack<Animal>>();

    // ���� ��
    public ChestList[] chestLists;
    public NpcList[] npcLists;
    public Dictionary<string, float[]> seedInfors = new Dictionary<string, float[]>();
    public Shop shop;
    public GroundInfor groundInfor;
    public int[] groundSize;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        canvas = GameObject.Find("Canvas").transform;
        resourcesManager = GameObject.Find("ResourcesManager").GetComponent<ResourcesManager>();
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        itemEvent = GameObject.Find("ItemEvent").GetComponent<ItemEvent>();
        seedsParent = GameObject.Find("Seeds").transform;

        panel_loading = canvas.Find("Loading").gameObject;
        panel_inventory = canvas.Find("Inventory").gameObject;
        panel_chest = canvas.Find("Chest").gameObject;
        panel_quickSlots = canvas.Find("Quick").gameObject;
        panel_shop = canvas.Find("Shop").gameObject;
        panel_npcDialogue = canvas.Find("NpcDialogue").gameObject;
        healthbar = canvas.Find("PlayerPanel").Find("Healthbar").GetComponent<Scrollbar>();
        feedbar = canvas.Find("PlayerPanel").Find("Feedbar").GetComponent<Scrollbar>();

        inventory = panel_inventory.GetComponent<Inventory>();
        quickSlot = panel_quickSlots.GetComponent<QuickSlot>();

        panel_loading.SetActive(true);

        StartCoroutine(SetInit());
    }

    IEnumerator SetInit()
    {
        yield return new WaitForSeconds(1.0f);

        // ���� ���� �ҷ�����
        SeedInfor seedInforsClass = JsonUtility.FromJson<SeedInfor>(resourcesManager.datas_env["SeedInfor"]);
        foreach(string seedInfor in seedInforsClass.seedInfor)
        {
            string[] cut1 = seedInfor.Split(" "); // 0 : ���� �̸�, 1 : �ð� ����
            float[] cut2 = Array.ConvertAll(cut1[1].Split(","), (x) => float.Parse(x));
            seedInfors.Add(cut1[0], cut2);
        }

        // �÷��̾� ���� �ҷ�����
        playerResource = JsonUtility.FromJson<PlayerResource>(resourcesManager.datas_player["PlayerResource"]);

        // ���� ���� �ҷ�����
        ChestListParent chestListParent = JsonUtility.FromJson<ChestListParent>(resourcesManager.datas_env["ChestInfor"]);
        chestLists = chestListParent.chest;
        foreach (ChestList chestList in chestLists)
        {
            Chest chest = GameObject.Find("Objects").transform.Find(chestList.name).GetComponent<Chest>();
            nowOpenChest = chest;
            chest.SetInit();
        }
        nowOpenChest = null;

        // Npc ���� �ҷ�����
        NpcListParent npcListParent = JsonUtility.FromJson<NpcListParent>(resourcesManager.datas_env["NpcInfor"]);
        npcLists = npcListParent.npc;

        // ���� �⺻����
        for (int i = 0; i < 2; i++)
            canMeetingAnimal.Add(new Stack<Animal>());

        inventory.SetInit();
        quickSlot.SetInit();
        objectManager.SetInit();
        player.SetInit();
        
        LoadGround();

        // �ε� �г� ���
        panel_loading.SetActive(false);

        isStart = true;
    }
    
    void LoadGround()
    {
        // y���� �������� �����ϱ� ������ json �Ľ� �� ��ȯ�������
        GroundInforParent groundInforParent = JsonUtility.FromJson<GroundInforParent>(resourcesManager.datas_player["Ground"]);
        groundInfor = groundInforParent.groundInfor;
        Debug.Log(groundInfor.size);

        groundSize = Array.ConvertAll(groundInfor.size.Split(" "), (a) => int.Parse(a)); // x, y

        // ����� obstacle Ÿ�� ����
        for (int i = 0; i < groundInfor.obstaclePos.Length; i++)
        {
            string[] tileInfor = groundInfor.obstaclePos[i].Split(' '); // x, y, tileName
            Vector3Int pos = new Vector3Int(int.Parse(tileInfor[0]), -int.Parse(tileInfor[1]));

            player.obstacleTile.SetTile(pos, resourcesManager.tiles[tileInfor[2]]);
        }

        // ����� ground Ÿ�� ����
        for (int i = 0; i < groundInfor.groundPos.Length; i++)
        {
            string[] tileInfor = groundInfor.groundPos[i].Split(' '); // x, y, tileName
            Vector3Int pos = new Vector3Int(int.Parse(tileInfor[0]), -int.Parse(tileInfor[1]));

            player.groundTile.SetTile(pos, resourcesManager.tiles[tileInfor[2]]);
        }

        // ��ġ ������ Ÿ�� ����
        groundInfor.prohibitList = new bool[groundSize[0], groundSize[1]];
        for (int i = 0; i < groundInfor.prohibitPos.Length; i++)
        {
            int[] prohibitInfor = Array.ConvertAll(groundInfor.prohibitPos[i].Split(' '), (a) => int.Parse(a)); // x, y

            groundInfor.prohibitList[prohibitInfor[0], prohibitInfor[1]] = true;
        }

        // �⺻ Ÿ�� ����
        for (int i = 0; i < groundSize[0]; i++)
        {
            for(int j = 0; j < groundSize[1]; j++)
            {
                if (player.groundTile.GetTile(new Vector3Int(i, -j)) == null && !groundInfor.prohibitList[i, j])
                {
                    player.groundTile.SetTile(new Vector3Int(i, -j), resourcesManager.tiles["Grass_Basic"]);
                }
            }
        }

        // �ɾ��� ���� ����
        for(int i = 0; i < groundInfor.seedPos.Length; i++)
        {
            string[] seedInfor = groundInfor.seedPos[i].Split(" "); // seedName, seedGrowIndex, x, y

            Vector3 pos = new Vector3(int.Parse(seedInfor[2]), int.Parse(seedInfor[3]));
            Seed seed = player.playerEvent.CreateSeed(seedInfor[0], int.Parse(seedInfor[1]), pos);
        }
    }

    public void QuickSlotEvent(int index)
    {
        if (quickSlot.img_quickSlots[index].color.g < 1) // ������ ���
        {
            if(nowQuickItem != null)
            {
                ItemImg item = img_nowQuickSlot.transform.GetChild(0).GetComponent<ItemImg>();

                if (item.CompareTag("Fruit") || item.CompareTag("Drink"))
                {
                    itemEvent.EatFood(item.originalName);
                }
            }
        }
        else // �ƴ϶�� ����
        {
            quickSlot.img_quickSlots[index].color = new Color(1, 0.2f, 0.2f, 1);
            
            if (img_nowQuickSlot != null) img_nowQuickSlot.color = new Color(1, 1, 1, 1);
            img_nowQuickSlot = quickSlot.img_quickSlots[index];

            ItemImg item = img_nowQuickSlot.transform.GetChild(0).GetComponent<ItemImg>();
            nowQuickItem = item;

            player.playerEvent.PreviewInstallBlock(nowQuickItem, player.frontPos, true);
        }

    }

    // �г� ���ΰ�ħ
    public void InstallItemOnPanel(List<ItemImg> items, Transform[][] panel_items)
    {
        int index = 0;

        for (int i = 0; i < panel_items.GetLength(0); i++)
        {
            for (int j = 0; j < panel_items[i].Length; j++)
            {
                ResetItemImg(panel_items[i][j]);

                // �������� �ִٸ�
                if (items.Count > index)
                {
                    ItemImg myItem = items[index++];
                    myItem.InstallOnPanel(myItem.transform, panel_items[i][j]);
                }
            }
        }

        //
        bool isRefreshQuick = isStart && panel_items[0][0].parent.name.Equals("Quick");
        if (isRefreshQuick)
        {
            if (img_nowQuickSlot.transform.childCount > 1)// ���� �����Կ� �������� �ִٸ�
                nowQuickItem = img_nowQuickSlot.transform.GetChild(0).GetComponent<ItemImg>();
            else if(img_nowQuickSlot.transform.childCount <= 1) // ���� �����Կ� �������� ���ٸ�
                nowQuickItem = null;

            player.playerEvent.PreviewInstallBlock(nowQuickItem, player.GetNowFrontPos(), true); // �̸����� ����
        }
    }

    public void ResetItemImg(Transform paenl_item)
    {
        if (paenl_item.childCount > 1)
        {
            paenl_item.GetChild(0).parent = null;
        }
        paenl_item.Find("text_Count").gameObject.SetActive(false);
    }

    // ������ ȹ�� �� ���
    public ItemImg SetItem(List<ItemImg> items, string name, string type, int count, bool useDurability)
    {
        Transform[][] items_Panel = null;

        switch (type)
        {
            case "Chest":
                items_Panel = nowOpenChest.panel_items;
                break;
            case "Inventory":
                items_Panel = inventory.panel_items;
                break;
            case "Quick":
                items_Panel = quickSlot.panel_items;
                break;
        }

        int itemIndex = CheckOverlap(items, GetGameObjectName(name, type));
        ItemImg item = null;
        
        if(itemIndex < 0)
        {
            // ������ �г��� �� á�� ��
            if (items.Count == items_Panel.Length * items_Panel[0].Length) return null;

            item = GameObject.Find(GetGameObjectName(name, type)).GetComponent<ItemImg>();

            // ������
            if (item.GetComponent<Durability>() != null)
                item.GetComponent<Durability>().SetInit();

            item = item.GetComponent<ItemImg>();
            item.count = count;
            items.Add(item);
        }
        else
        {
            item = items[itemIndex];

            bool activeDurability = false;

            // useDurability : �������� ����ϴ���, �ٷ� ������ ������ �� üũ
            if (items[itemIndex].GetComponent<Durability>() != null && useDurability)
            {
                activeDurability = items[itemIndex].GetComponent<Durability>().SetDurability(-1);
            }

            // �������� �ƿ� ���ų� �Ҹ��ٸ�
            if (!activeDurability)
            {
                items[itemIndex].count += count;
                if (items[itemIndex].count <= 0)
                {
                    //if (type.Equals("Quick")) nowQuickItem = null;

                    items[itemIndex].transform.parent = null;
                    items.RemoveAt(itemIndex);
                }
            }
        }

        return item;
    }

    public int CheckOverlap(List<ItemImg> items, string originalItemName)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].name.Equals(originalItemName)) return i;
        }
        return -1;
    }

    public void SetHealth(float value)
    {
        float healValue = value;

        // ȸ���� ü���� �ִ� ü�º��� ũ�� ���� ü�� ��� ȸ��
        if (player.nowHealth + value > player.maxHealth)
            healValue = player.maxHealth - player.nowHealth;

        player.nowHealth += healValue;
        healthbar.size = player.nowHealth / player.maxHealth;
    }

    public void SetFeed(float value)
    {
        float feedValue = value;

        // ȸ���� ��ⷮ�� �ִ� ��ⷮ���� ũ�� ���� ��ⷮ ��� ȸ��
        if (player.nowFeed + value > player.maxFeed)
            feedValue = player.maxFeed - player.nowFeed;

        player.nowFeed += feedValue;
        feedbar.size = player.nowFeed / player.maxFeed;
    }

    public void SetMoney(int value)
    {
        playerResource.money += value;
        inventory.text_money.text = $"Money : {playerResource.money} $";

        // ���� ȹ�� �ִϸ��̼�
        player.ShowAniEvent(player.coinAni);
    }

    public static bool IsGetAniState(Animator animator, string name, float time)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(name) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= time;
    }

    public static void PlaySound(AudioSource audioSource, AudioClip clip, bool isLoop)
    {
        audioSource.clip = clip;
        audioSource.loop = isLoop;
        audioSource.Play();
    }

    public static string GetGameObjectName(string name, string type)
    {
        return $"{name}(Clone)_{type}";
    }

    public static string GetPrefebName(string name)
    {
        return $"{name}(Clone)";
    }

    public void Save()
    {
        // ��ΰ� ���ٸ� (�Ŀ� ������ ����)
        if (!Directory.Exists(Path.Combine(Application.dataPath, "Datas_Save")))
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "Datas_Save"));

        // �� ����
        MyQuickSlot myQuickSlot = new MyQuickSlot();
        myQuickSlot.items = GetStringData(quickSlot.items);

        // �κ��丮
        ItemListParent itemListParent = new ItemListParent();
        itemListParent.items = new ItemList[inventory.myItems.Count];
        for (int i = 0; i < inventory.myItems.Count; i++)
        {
            ItemList itemList = new ItemList();
            itemList.item = GetStringData(inventory.myItems[i]);

            itemListParent.items[i] = itemList;
        }

        // �÷��̾� ����

        // ���� ����
        ChestListParent chestListParent = new ChestListParent();
        chestListParent.chest = new ChestList[chests.Count];
        for (int i = 0; i < chests.Count; i++)
        {
            ChestList chestList = new ChestList();
            chestList.name = chests[i].name;
            chestList.item = GetStringData(chests[i].items);

            chestListParent.chest[i] = chestList;
        }

        // Ÿ�� ����
        List<string> obstaclePos = new List<string>();
        List<string> groundPos = new List<string>();
        List<string> seedPos = new List<string>();

        for (int i = 0; i < groundSize[0]; i++)
        {
            for(int j = 0; j < groundSize[1]; j++)
            {
                if(player.obstacleTile.GetTile(new Vector3Int(i, -j)) != null)
                {
                    string tileName = player.obstacleTile.GetTile(new Vector3Int(i, -j)).name;
                    obstaclePos.Add($"{i} {j} {tileName}");
                }
            }
        }
        for (int i = 0; i < groundSize[0]; i++)
        {
            for (int j = 0; j < groundSize[1]; j++)
            {
                if (player.groundTile.GetTile(new Vector3Int(i, -j)) != null && 
                    !player.groundTile.GetTile(new Vector3Int(i, -j)).name.Equals("Grass_Basic"))
                {
                    string tileName = player.groundTile.GetTile(new Vector3Int(i, -j)).name;
                    groundPos.Add($"{i} {j} {tileName}");
                }
            }
        }
        for(int i = 0; i < seedsParent.childCount; i++)
        {
            Seed seed = seedsParent.GetChild(i).GetComponent<Seed>();
            seedPos.Add($"{seed.seedName} {seed.growCount} {seed.transform.position.x} {seed.transform.position.y}");
        }

        groundInfor.obstaclePos = obstaclePos.ToArray();
        groundInfor.groundPos = groundPos.ToArray();
        groundInfor.seedPos = seedPos.ToArray();

        GroundInforParent groundInforParent = new GroundInforParent
        {
            groundInfor = groundInfor
        };

        PlayerInfor playerInfor = new PlayerInfor
        {
            itemListParent = itemListParent,
            myQuickSlot = myQuickSlot,
            playerResource = playerResource,
            groundInforParent = groundInforParent
        };
        string text = JsonUtility.ToJson(playerInfor);

        //DatabaseReference db = FirebaseDatabase.DefaultInstance.RootReference;
        //db.Child(resourcesManager.loginManager.userId).SetValueAsync(text);

        //SaveFile("MyQuickSlot.json", quickSlotText);
        //SaveFile("MyItem.json", inventoryText);
        //SaveFile("Player.json", playerInforText);
        //SaveFile("Chest.json", chestText);
        //SaveFile("Ground.json", groundText);
    }

    //public static void SaveFile(string jsonName, string tarText)
    //{
    //    if (!File.Exists(Path.Combine(Application.dataPath, "Datas_Save", jsonName)))
    //    {
    //        using (FileStream fs = File.Create(jsonName))
    //            fs.Dispose();
    //    }

    //    File.WriteAllText(Path.Combine(Application.dataPath, "Datas_Save", jsonName), string.Empty);
    //    File.WriteAllText(Path.Combine(Application.dataPath, "Datas_Save", jsonName), tarText);
    //}

    public static string[] GetStringData(List<ItemImg> items)
    {
        string[] textArr = new string[items.Count];

        for (int i = 0; i < textArr.Length; i++)
        {
            string text = $"{items[i].originalName.Substring(0, items[i].originalName.Length - 3)} {items[i].count}";
            textArr[i] = text;
        }

        return textArr;
    }

    void OnApplicationQuit()
    {
        if (isStart)
        {
            Save();
        }
    }
}
