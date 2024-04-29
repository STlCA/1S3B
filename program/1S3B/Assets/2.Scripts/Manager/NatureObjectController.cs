using Constants;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;
using static UnityEngine.Rendering.DebugUI;
using Random = UnityEngine.Random;

[System.Serializable]
public struct SaveSpawnData
{
    public List<Vector3Int> NatureCellPos;
    public List<SaveNatureData> NatureSaveData;

    public List<Vector3Int> TreeCellPos;
    public List<SaveTreeData> TreeSaveData;

    public List<Vector3Int> StoneCellPos;
    public List<SaveStoneData> StoneSaveData;
}

public class NatureData
{
    public GameObject natureObj;
    public SpriteRenderer natureRenderer;
    public SpriteResolver natureResolver;
    public int id;
    public string category;
    public string labelName;
    public bool isSpawn = false;

    public void Init(GameObject go, int id, string labelName, string season)
    {
        natureObj = go;
        natureRenderer = go.GetComponentInChildren<SpriteRenderer>();
        natureResolver = go.GetComponentInChildren<SpriteResolver>();

        isSpawn = true;
        this.labelName = labelName;
        this.id = id;
        category = season;

        natureResolver.SetCategoryAndLabel(category, labelName);
    }

    public void Save(ref SaveNatureData data)
    {
        data.ID = id;
        data.LabelName = labelName;
        data.IsSpawn = isSpawn;
        data.CategoryName = category;
    }
    public void Load(SaveNatureData data, GameObject go)
    {
        natureObj = go;
        natureRenderer = go.GetComponentInChildren<SpriteRenderer>();
        natureResolver = go.GetComponentInChildren<SpriteResolver>();

        id = data.ID;
        isSpawn = data.IsSpawn;
        category = data.CategoryName;
        labelName = data.LabelName;

        natureResolver.SetCategoryAndLabel(data.CategoryName, data.LabelName);
    }
}

[System.Serializable]
public struct SaveNatureData
{
    public int ID;
    public string CategoryName;
    public string LabelName;
    public bool IsSpawn;
}


public class TreeData
{
    public GameObject treeObj;
    public SpriteResolver treeResolver;
    public Animator animator;
    public int treeType;
    public string currentLabel;
    public float count = 0;
    public int cutConut = 10;
    public int maxCount = 15;
    public bool isSpawn = false;
    public bool itemDrop = false;
    public bool isPoint = false;

    public void Save(ref SaveTreeData data)
    {
        data.TreeType = treeType;
        data.Count = count;
        data.IsSpawn = isSpawn;
        data.IsPoint = isPoint;
        data.ItemDrop = itemDrop;
        data.CurrentLabel = currentLabel;
    }
    public void Load(SaveTreeData data, GameObject go, RuntimeAnimatorController _animator)
    {
        treeType = data.TreeType;
        count = data.Count;
        isSpawn = data.IsSpawn;
        itemDrop = data.ItemDrop;
        isPoint = data.IsPoint;
        currentLabel = data.CurrentLabel;

        treeObj = go;
        treeResolver = treeObj.GetComponentInChildren<SpriteResolver>();
        animator = treeObj.GetComponentInChildren<Animator>();

        if (data.ItemDrop == true)
        {
            animator.runtimeAnimatorController = _animator;
            animator.enabled = false;
        }
        else
            animator.enabled = false;

        treeResolver.SetCategoryAndLabel(treeResolver.GetCategory(), data.CurrentLabel);
    }
}
[System.Serializable]
public struct SaveTreeData
{
    public int TreeType;
    public float Count;
    public bool IsSpawn;
    public bool ItemDrop;
    public bool IsPoint;
    public string CurrentLabel;
}

public class StoneData
{
    public GameObject stoneObj;
    public Animator animator;
    public float count = 0;
    public int maxCount = 3;
    public bool isSpawn = false;
    public bool isPoint = false;
    public StoneType type;

    public void Save(ref SaveStoneData data)
    {
        data.Count = count;
        data.IsSpawn = isSpawn;
        data.IsPoint = isPoint;
        data.Type = (int)type;
    }

    public void Load(SaveStoneData data, GameObject go)
    {
        count = data.Count;
        isSpawn = data.IsSpawn;
        isPoint = data.IsPoint;
        type = (StoneType)data.Type;

        stoneObj = go;
        animator = go.GetComponentInChildren<Animator>();

        go.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel("Stone", ((int)type).ToString());
    }
}

[System.Serializable]
public struct SaveStoneData
{
    public float Count;
    public bool IsSpawn;
    public bool IsPoint;
    public int Type;
}

//=============================================^ Class Struct

public class NatureObjectController : Manager
{
    private Player player;
    private Inventory inventory;
    private TileManager tileManager;
    private TargetSetting targetSetting;
    private AnimationController animationController;
    private DayCycleHandler dayCycleHandler;
    private SoundSystemManager soundManager;

    [Header("Range")]
    public Tilemap interactableMap;
    public Transform[] farmRange;
    public Transform[] forestRange;
    public Transform[] quarryRange;

    [Header("Point")]
    public GameObject naturePointObject;
    private Transform[] naturePoint;
    public GameObject treePointObject;
    private Transform[] treePoint;
    public GameObject stonePointObject;
    private Transform[] stonePoint;

    [Header("Nature")]
    public GameObject naturePrefab;
    public SpriteLibraryAsset natureLibrary;

    [Header("Tree")]
    public GameObject tree1Prefab;
    public GameObject tree2Prefab;
    public GameObject tree3Prefab;
    public RuntimeAnimatorController posAnimator;

    [Header("Stone")]
    public GameObject stonePrefab;

    [Header("Item")]
    public GameObject dropItemPrefab;

    public Dictionary<Vector3Int, NatureData> natureData = new();
    public Dictionary<Vector3Int, TreeData> treeData = new();
    public Dictionary<Vector3Int, StoneData> stoneData = new();

    private Vector3Int saveTarget = new();
    private ItemDatabase itemDatabase;

    private void Awake()
    {
        if (naturePointObject != null)
            naturePoint = naturePointObject.GetComponentsInChildren<Transform>();
        if (treePointObject != null)
            treePoint = treePointObject.GetComponentsInChildren<Transform>();
        if (stonePointObject != null)
            stonePoint = stonePointObject.GetComponentsInChildren<Transform>();
    }

    public override void Init(GameManager gm)
    {
        base.Init(gm);

        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        animationController = gameManager.AnimationController;
        player = gameManager.Player;
        dayCycleHandler = gameManager.DayCycleHandler;
        soundManager = gameManager.SoundManager;
    }

    private void Start()
    {
        inventory = player.Inventory;
        itemDatabase = gameManager.DataManager.itemDatabase;

        animationController.useAnimEnd += CutTreeTime;
        animationController.useAnimEnd += DestroyTree;
        animationController.useAnimEnd += DestroyStone;

        dayCycleHandler.changeSeasonAction += SpriteChange;
        dayCycleHandler.changeSeasonAction += ResetNature;
        dayCycleHandler.changeSeasonAction += SeasonSpawn;
    }

    private void StartSetting()
    {
        if (naturePoint != null)
        {
            foreach (Transform go in naturePoint)
            {
                if (go.position == Vector3.zero)
                    continue;

                Vector3Int goCellPos = tileManager.baseGrid.WorldToCell(go.position);

                NatureData tempData = new NatureData();

                natureData.Add(goCellPos, tempData);
            }
        }

        if (stonePoint != null)
        {
            foreach (Transform go in stonePoint)
            {
                if (go.position == Vector3.zero)
                    continue;

                Vector3Int goCellPos = tileManager.baseGrid.WorldToCell(go.position);

                StoneData tempData = new StoneData();
                tempData.isPoint = true;

                stoneData.Add(goCellPos, tempData);
            }
        }

        if (treePoint != null)
        {
            foreach (Transform go in treePoint)
            {
                if (go.position == Vector3.zero)
                    continue;

                Vector3Int goCellPos = tileManager.baseGrid.WorldToCell(go.position);

                TreeData tempData = new TreeData();
                tempData.isPoint = true;

                treeData.Add(goCellPos, tempData);
            }
        }
    }

    private void StartSpawn()
    {
        PointSpawnTree(100);
        PointSpawnStone(100);

        RangeSpawnTree(20, SpawnPlace.Farm);
        RangeSpawnStone(20, SpawnPlace.Farm);

        RangeSpawnTree(40, SpawnPlace.UpForest);
        RangeSpawnStone(20, SpawnPlace.UpForest);

        RangeSpawnTree(40, SpawnPlace.DownForest);
        RangeSpawnStone(20, SpawnPlace.DownForest);

        RangeSpawnStone(40, SpawnPlace.Quarry);
    }

    private void SeasonSpawn(Season season)
    {
        if (season == Season.Spring)
        {
            RangeSpawnTree(20, SpawnPlace.Farm);
            RangeSpawnStone(20, SpawnPlace.Farm);

            RangeSpawnTree(40, SpawnPlace.UpForest);
            RangeSpawnStone(20, SpawnPlace.UpForest);

            RangeSpawnTree(40, SpawnPlace.DownForest);
            RangeSpawnStone(20, SpawnPlace.DownForest);

            RangeSpawnStone(40, SpawnPlace.Quarry);
        }

        RangeSpawnTree(10, SpawnPlace.Farm);
        RangeSpawnStone(10, SpawnPlace.Farm);

        RangeSpawnTree(20, SpawnPlace.UpForest);
        RangeSpawnStone(10, SpawnPlace.UpForest);

        RangeSpawnTree(20, SpawnPlace.DownForest);
        RangeSpawnStone(10, SpawnPlace.DownForest);

        RangeSpawnStone(20, SpawnPlace.Quarry);
    }


    public void SpawnNature()
    {
        float percentage = 20;
        float randomPoint;
        int random;

        foreach (var (cell, tempdData) in natureData)
        {
            if (tempdData.isSpawn == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint <= percentage)
                {
                    GameObject go = Instantiate(naturePrefab);
                    go.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);

                    string season = dayCycleHandler.currentSeason.ToString();
                    IEnumerable<string> names = natureLibrary.GetCategoryLabelNames(season);

                    random = Random.Range(0, names.Count()) + 1;
                    string id = "5" + ((dayCycleHandler.currentDay / 28) % 4).ToString() + "0" + (random).ToString();

                    tempdData.Init(go, int.Parse(id), random.ToString(), season);
                }
            }
        }
    }

    private void ResetNature(Season season)
    {
        foreach (var (cell, data) in natureData)
        {
            Destroy(data.natureObj);
            natureData[cell].isSpawn = false;
        }
    }

    private GameObject RandomTree(ref int treeType, bool isLoad = false)
    {
        int random;

        if (isLoad)
            random = treeType;
        else
        {
            random = Random.Range(1, 4);
            treeType = random;
        }

        switch (random)
        {
            case 1:
                return Instantiate(tree1Prefab);
            case 2:
                return Instantiate(tree2Prefab);
            case 3:
                return Instantiate(tree3Prefab);
        }

        return null;
    }


    public void PointSpawnTree(float percent)
    {
        float percentage = percent;
        float randomPoint;

        foreach (var (cell, tempData) in treeData)
        {
            if (tempData.isSpawn == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    tempData.treeObj = RandomTree(ref tempData.treeType);

                    tempData.isSpawn = true;
                    tempData.treeObj.transform.position = (Vector3)cell + new Vector3(0.5f, 0.2f, 0);
                    tempData.treeResolver = tempData.treeObj.GetComponentInChildren<SpriteResolver>();
                    tempData.animator = tempData.treeObj.GetComponentInChildren<Animator>();
                    tempData.currentLabel = dayCycleHandler.currentSeason.ToString();

                    ChangeCategoryLabel(ref tempData.animator, ref tempData.treeResolver, tempData.currentLabel);
                }
            }
        }
    }

    public void PointSpawnStone(float percent)
    {
        float percentage = percent;
        float randomPoint;

        foreach (var (cell, tempData) in stoneData)
        {
            if (tempData.isSpawn == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    tempData.stoneObj = Instantiate(stonePrefab);
                    tempData.isSpawn = true;
                    tempData.stoneObj.transform.position = (Vector3)cell + new Vector3(0.5f, 0.2f, 0);
                    tempData.animator = tempData.stoneObj.GetComponentInChildren<Animator>();

                    int random = Random.Range(1, 11);
                    if (random <= 2)
                        tempData.type = StoneType.RANDOMSTONE;
                    else
                        tempData.type = StoneType.STONE;

                    tempData.stoneObj.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel("Stone", ((int)tempData.type).ToString());
                }
            }
        }
    }

    private Vector3 RandomXY(SpawnPlace type)
    {
        float randomX;
        float randomY;

        if (type == SpawnPlace.UpForest)
        {
            randomX = Random.Range(forestRange[0].position.x, forestRange[1].position.x);
            randomY = Random.Range(forestRange[0].position.y, forestRange[1].position.y);
        }
        else if (type == SpawnPlace.DownForest)
        {
            randomX = Random.Range(forestRange[2].position.x, forestRange[3].position.x);
            randomY = Random.Range(forestRange[2].position.y, forestRange[3].position.y);
        }
        else if (type == SpawnPlace.Farm)
        {
            randomX = Random.Range(farmRange[0].position.x, farmRange[1].position.x);
            randomY = Random.Range(farmRange[0].position.y, farmRange[1].position.y);
        }
        else if (type == SpawnPlace.Quarry)
        {
            randomX = Random.Range(quarryRange[0].position.x, quarryRange[1].position.x);
            randomY = Random.Range(quarryRange[0].position.y, quarryRange[1].position.y);
        }
        else
            return Vector3.zero;

        return new Vector3(randomX, randomY);
    }

    private bool SpawnCheck(Vector3Int target)
    {
        if (stoneData.ContainsKey(target) == true)
            return false;
        if (treeData.ContainsKey(target) == true)
            return false;
        if (tileManager.cropData.ContainsKey(target) == true)
            return false;
        if (natureData.ContainsKey(target) == true)
            return false;
        if (interactableMap.GetTile(target) == null)
            return false;

        if (tileManager.groundData.ContainsKey(target) == true)
            tileManager.DestroyGroundData(target);

        return true;
    }

    public void RangeSpawnTree(int spawnCount, SpawnPlace type)
    {
        Vector3Int randomPos;

        for (int i = 0; i < spawnCount;)
        {
            if (treeData.Count >= 150)
                return;

            randomPos = tileManager.baseGrid.WorldToCell(RandomXY(type));

            if (SpawnCheck(randomPos) == true)
            {
                TreeData newTree = new();

                newTree.treeObj = RandomTree(ref newTree.treeType);

                newTree.isSpawn = true;
                newTree.treeObj.transform.position = (Vector3)randomPos + new Vector3(0.5f, 0.2f, 0);
                newTree.treeResolver = newTree.treeObj.GetComponentInChildren<SpriteResolver>();
                newTree.animator = newTree.treeObj.GetComponentInChildren<Animator>();
                newTree.currentLabel = dayCycleHandler.currentSeason.ToString();

                ChangeCategoryLabel(ref newTree.animator, ref newTree.treeResolver, newTree.currentLabel);

                treeData.Add(randomPos, newTree);
                ++i;
            }
        }
    }

    public void RangeSpawnStone(int spawnCount, SpawnPlace type)
    {
        Vector3Int randomPos;

        for (int i = 0; i < spawnCount;)
        {
            if (stoneData.Count >= 200)
                return;

            randomPos = tileManager.baseGrid.WorldToCell(RandomXY(type));

            if (SpawnCheck(randomPos) == true)
            {
                StoneData newStone = new();
                newStone.stoneObj = Instantiate(stonePrefab);
                newStone.isSpawn = true;
                newStone.stoneObj.transform.position = (Vector3)randomPos + new Vector3(0.5f, 0.2f, 0);
                newStone.animator = newStone.stoneObj.GetComponentInChildren<Animator>();

                int random = Random.Range(1, 11);
                if (random <= 2)
                    newStone.type = StoneType.RANDOMSTONE;
                else
                    newStone.type = StoneType.STONE;

                newStone.stoneObj.GetComponentInChildren<SpriteResolver>().SetCategoryAndLabel("Stone", ((int)newStone.type).ToString());

                stoneData.Add(randomPos, newStone);
                ++i;
            }
        }
    }

    public bool IsPickUpNature(Vector3Int target)
    {
        if (targetSetting.PlayerBoundCheck() == false)
            return false;

        if (natureData.ContainsKey(target) && natureData[target].isSpawn == false)
            return false;

        return natureData.ContainsKey(target) && natureData[target].isSpawn == true;
    }

    public void PickUpNature(Vector3Int target, Vector2 pos)//플레이어가 마우스를바라보는 방향
    {
        Sprite pickUpSprite = natureData[target].natureRenderer.sprite;
        animationController.PickUpAnim(target, pos, pickUpSprite);

        natureData[target].isSpawn = false;

        ItemInfo itemInfo = itemDatabase.GetItemByKey(natureData[target].id);
        Item item = new Item();
        item.ItemInfo = itemInfo;
        inventory.AddItem(item);

        Destroy(natureData[target].natureObj);
    }

    public bool IsFelling(Vector3Int target)
    {
        if (targetSetting.PlayerBoundCheck() == false)
            return false;

        if (treeData.ContainsKey(target) && treeData[target].isSpawn == false)
            return false;

        return treeData.ContainsKey(target) && treeData[target].isSpawn == true;
    }

    public void Felling(Vector3Int target)
    {
        if (targetSetting.PlayerBoundCheck() == false)
            return;

        treeData[target].count++;
        Vector3 direction = treeData[target].treeObj.transform.position - player.transform.position;

        if (treeData[target].animator.enabled == false)
            treeData[target].animator.enabled = true;

        treeData[target].animator.SetTrigger("isFelling");
        treeData[target].animator.SetTrigger(dayCycleHandler.currentSeason.ToString());
        treeData[target].animator.SetFloat("inputX", direction.x);
        treeData[target].animator.SetFloat("inputY", direction.y);

        saveTarget = target;
    }

    public bool IsMining(Vector3Int target)
    {
        if (targetSetting.PlayerBoundCheck() == false)
            return false;

        if (stoneData.ContainsKey(target) && stoneData[target].isSpawn == false)
            return false;

        return stoneData.ContainsKey(target) && stoneData[target].isSpawn == true;
    }

    public void Mining(Vector3Int target)
    {
        if (targetSetting.PlayerBoundCheck() == false)
            return;

        stoneData[target].count++;
        Vector3 direction = stoneData[target].stoneObj.transform.position - player.transform.position;

        if (stoneData[target].animator.enabled == false)//뽀사지는 애니메이션을 넣으려고
            stoneData[target].animator.enabled = true;

        stoneData[target].animator.SetTrigger("isFelling");//TODO
        stoneData[target].animator.SetFloat("inputX", direction.x);
        stoneData[target].animator.SetFloat("inputY", direction.y);

        saveTarget = target;
    }

    public void CutTreeTime(bool value)//애니메이션이 끝나는 타이밍에 호출
    {
        if (saveTarget != Vector3Int.zero && treeData.ContainsKey(saveTarget) == true)
        {
            if (treeData[saveTarget].itemDrop == false && treeData[saveTarget].count >= treeData[saveTarget].cutConut)
            {
                soundManager.GameAudioClipPlay((int)MainAudioClip.CutTree);

                //treeData[target].animator.SetTrigger("isFellied");
                treeData[saveTarget].itemDrop = true;
                treeData[saveTarget].animator.runtimeAnimatorController = posAnimator;
                treeData[saveTarget].currentLabel = "0";

                ChangeCategoryLabel(ref treeData[saveTarget].animator, ref treeData[saveTarget].treeResolver, "0");

                treeData[saveTarget].treeObj.GetComponentInChildren<PolygonCollider2D>().enabled = false;

                Vector3 spawItemPos = (player.transform.position - treeData[saveTarget].treeObj.transform.position).normalized;
                Vector3 dropPos = new();
                if (spawItemPos.x < 0)
                    dropPos.x = 1f;
                else if (spawItemPos.x > 0)
                    dropPos.x = -1f;

                DropItem(treeData[saveTarget].treeObj.transform.position + dropPos, 10, (int)DropItemType.Wood);
            }
        }
    }

    public void DestroyTree(bool value)
    {
        if (saveTarget != Vector3Int.zero && treeData.ContainsKey(saveTarget) == true)
        {
            if (treeData[saveTarget].count >= treeData[saveTarget].maxCount)
            {
                DropItem(treeData[saveTarget].treeObj.transform.position, 5, (int)DropItemType.Wood);
                Destroy(treeData[saveTarget].treeObj);
                treeData[saveTarget].isSpawn = false;
                treeData[saveTarget].itemDrop = false;
                treeData[saveTarget].count = 0;

                if (treeData[saveTarget].isPoint == false)
                    treeData.Remove(saveTarget);

                saveTarget = Vector3Int.zero;
            }
        }
    }

    public void DestroyStone(bool value)
    {
        if (saveTarget != Vector3Int.zero && stoneData.ContainsKey(saveTarget) == true)
        {
            if (stoneData[saveTarget].count >= stoneData[saveTarget].maxCount)
            {
                int count = Random.Range(1, 4);

                if (stoneData[saveTarget].type == StoneType.STONE)
                    DropItem(stoneData[saveTarget].stoneObj.transform.position, count, (int)DropItemType.Stone);
                else
                    DropItem(stoneData[saveTarget].stoneObj.transform.position, count, DropItemType.Stone);

                Destroy(stoneData[saveTarget].stoneObj);
                stoneData.Remove(saveTarget);

                saveTarget = Vector3Int.zero;
            }
        }
    }

    private void DropItem(Vector3 target, int count, int ID)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject go = Instantiate(dropItemPrefab);
            go.GetComponentInChildren<SpriteRenderer>().sprite = itemDatabase.GetItemByKey(ID).SpriteList[0];
            go.GetComponent<DropItem>().id = ID;
            go.transform.position = new Vector3(target.x, target.y + 0.5f);
        }
    }
    private void DropItem(Vector3 target, int count, DropItemType type)
    {
        for (int i = 0; i < count; ++i)
        {
            ItemInfo item = RandomItem(type);

            GameObject go = Instantiate(dropItemPrefab);
            go.GetComponentInChildren<SpriteRenderer>().sprite = item.SpriteList[0];
            go.GetComponent<DropItem>().id = item.ID;
            go.transform.position = new Vector3(target.x, target.y + 0.5f);
        }
    }
    private ItemInfo RandomItem(DropItemType type)
    {
        ItemInfo[] spawnItem;

        switch (type)
        {
            case DropItemType.Stone:
            default:
                spawnItem = new ItemInfo[5];//돌추가되면 갯수바까야함
                for (int i = 0; i < spawnItem.Length; ++i)
                {
                    spawnItem[i] = itemDatabase.GetItemByKey(int.Parse("401" + (i + 1).ToString()));
                }
                break;
        }

        int dropIndex = 0;
        float total = 0;
        float[] itemPercent = new float[spawnItem.Length];

        for (int i = 0; i < spawnItem.Length; i++)
        {
            float percent = spawnItem[i].DropPercent;
            itemPercent[i] = percent;
            total += percent;
        }

        float randomPoint = Random.value * total;

        for (int i = 0; i < itemPercent.Length; i++)
        {
            if (randomPoint <= itemPercent[i])
            {
                dropIndex = i;
                break;
            }
            else
                randomPoint -= itemPercent[i];
        }

        return spawnItem[dropIndex];
    }

    public void SpriteChange(Season current)
    {
        foreach (var (cell, temp) in treeData)
        {
            if (temp.animator == null)
                continue;

            ChangeCategoryLabel(ref temp.animator, ref temp.treeResolver, current.ToString());
        }
    }

    private void ChangeCategoryLabel(ref Animator animator, ref SpriteResolver resolver, string current)
    {
        animator.enabled = false;
        string category = resolver.GetCategory();
        resolver.SetCategoryAndLabel(category, current);
    }

    //============================================================Save

    public void Save(ref SaveSpawnData data, bool isNew = false)
    {
        if (isNew)
        {
            StartSetting();
            StartSpawn();
        }

        data.NatureCellPos = new();
        data.NatureSaveData = new();

        foreach (var (key, value) in natureData)
        {
            data.NatureCellPos.Add(key);

            SaveNatureData saveData = new();
            value.Save(ref saveData);

            data.NatureSaveData.Add(saveData);
        }

        data.TreeCellPos = new();
        data.TreeSaveData = new();

        foreach (var (key, value) in treeData)
        {
            data.TreeCellPos.Add(key);

            SaveTreeData saveData = new();
            value.Save(ref saveData);

            data.TreeSaveData.Add(saveData);
        }

        data.StoneCellPos = new();
        data.StoneSaveData = new();

        foreach (var (key, value) in stoneData)
        {
            data.StoneCellPos.Add(key);

            SaveStoneData saveData = new();
            value.Save(ref saveData);

            data.StoneSaveData.Add(saveData);
        }
    }

    public void Load(SaveSpawnData data)
    {
        natureData = new();
        for (int i = 0; i < data.NatureSaveData.Count; i++)
        {
            NatureData newData = new();

            if (data.NatureSaveData[i].IsSpawn == true)
            {
                GameObject go = Instantiate(naturePrefab);
                go.transform.position = gameManager.TileManager.baseGrid.GetCellCenterWorld(data.NatureCellPos[i]);

                newData.Load(data.NatureSaveData[i], go);
            }

            natureData.Add(data.NatureCellPos[i], newData);
        }

        treeData = new();
        for (int i = 0; i < data.TreeSaveData.Count; i++)
        {
            TreeData newData = new();

            int temp = data.TreeSaveData[i].TreeType;

            if (data.TreeSaveData[i].IsSpawn == true)
            {
                GameObject go = RandomTree(ref temp, true);
                go.transform.position = data.TreeCellPos[i] + new Vector3(0.5f, 0.2f, 0);
                //go.transform.position = gameManager.TileManager.baseGrid.WorldToCell(data.TreeCellPos[i]) + new Vector3(0.5f, 0.2f, 0);

                newData.Load(data.TreeSaveData[i], go, posAnimator);
            }
            else
                newData.isPoint = true;

            treeData.Add(data.TreeCellPos[i], newData);
        }

        stoneData = new();
        for (int i = 0; i < data.StoneSaveData.Count; i++)
        {
            StoneData newData = new();

            if (data.StoneSaveData[i].IsSpawn == true)
            {
                GameObject go = Instantiate(stonePrefab);
                go.transform.position = data.StoneCellPos[i] + new Vector3(0.5f, 0.2f, 0);

                newData.Load(data.StoneSaveData[i], go);
            }
            else
                newData.isPoint = true;

            stoneData.Add(data.StoneCellPos[i], newData);
        }
    }

}


