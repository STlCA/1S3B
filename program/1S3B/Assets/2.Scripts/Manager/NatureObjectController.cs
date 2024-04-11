using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.ShaderGraph;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;
using Random = UnityEngine.Random;


[System.Serializable]
public class NatureData
{
    public GameObject natureObj;
    public SpriteRenderer natureRenderer;
    public SpriteResolver natureResolver;
    public bool isSpawn = false;
}

public class TreeData
{
    public GameObject treeObj;
    public SpriteResolver treeResolver;
    public Animator animator;
    public float count = 0;
    public int cutConut = 10;
    public int maxCount = 15;
    public bool isSpawn = false;
    public bool itemDrop = false;
    public bool isPoint = false;
}

public class StoneData
{
    public GameObject stoneObj;
    public Animator animator;
    public float count = 0;
    public int maxCount = 3;
    public bool isSpawn = false;
    public bool itemDrop = false;
}

public class NatureObjectController : Manager
{
    private Player player;
    private TileManager tileManager;
    private TargetSetting targetSetting;
    private AnimationController animationController;
    private DayCycleHandler dayCycleHandler;

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

    private Dictionary<Vector3Int, NatureData> natureData = new();
    private Dictionary<Vector3Int, TreeData> treeData = new();
    private Dictionary<Vector3Int, StoneData> stoneData = new();

    private Vector3Int saveTarget = new();
    private ItemDatabase itemDatabase;

    private void Start()
    {
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        animationController = gameManager.AnimationController;
        player = gameManager.Player;
        dayCycleHandler = gameManager.DayCycleHandler;
        itemDatabase = gameManager.DataManager.itemDatabase;

        if (naturePointObject != null)
            naturePoint = naturePointObject.GetComponentsInChildren<Transform>();
        if (treePointObject != null)
            treePoint = treePointObject.GetComponentsInChildren<Transform>();
        if (stonePointObject != null)
            stonePoint = stonePointObject.GetComponentsInChildren<Transform>();

        animationController.useAnimEnd += CutTreeTime;
        animationController.useAnimEnd += DestroyTree;
        animationController.useAnimEnd += DestroyStone;

        dayCycleHandler.changeSeasonAction += SpriteChange;

        StartSetting();
    }

    private void StartSetting()
    {
        if (naturePoint != null)
        {
            foreach (Transform go in naturePoint)
            {

                Vector3Int goCellPos = tileManager.baseGrid.WorldToCell(go.position);

                NatureData tempData = new NatureData();

                natureData.Add(goCellPos, tempData);
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

    public void SpawnNature()
    {
        float percentage = 50;
        float randomPoint;
        int random;

        foreach (var (cell, tempdData) in natureData)
        {
            if (tempdData.isSpawn == false && tileManager.croptData.ContainsKey(cell) == false && treeData.ContainsKey(cell) == false && stoneData.ContainsKey(cell) == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    //Init()화하기
                    tempdData.isSpawn = true;
                    tempdData.natureObj = Instantiate(naturePrefab);
                    tempdData.natureObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempdData.natureRenderer = tempdData.natureObj.GetComponent<SpriteRenderer>();
                    tempdData.natureResolver = tempdData.natureObj.GetComponent<SpriteResolver>();

                    string season = "Spring";

                    IEnumerable<string> names = natureLibrary.GetCategoryLabelNames(season);

                    random = Random.Range(0, names.Count());

                    tempdData.natureResolver.SetCategoryAndLabel(season, random.ToString());
                    //tempData.natureRenderer.sprite = natureLibrary.GetSprite(season, random.ToString());
                }
            }
        }
    }

    private GameObject RandomTree()
    {
        int random;

        random = Random.Range(1, 4);

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


    public void PointSpawnTree()
    {
        float percentage = 50;
        float randomPoint;

        foreach (var (cell, tempData) in treeData)
        {
            if (tempData.isSpawn == false && tileManager.croptData.ContainsKey(cell) == false && natureData.ContainsKey(cell) == false && stoneData.ContainsKey(cell) == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    tempData.treeObj = RandomTree();

                    tempData.isSpawn = true;
                    //tempData.treeObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempData.treeObj.transform.position = (Vector3)cell + new Vector3(0.5f, 0.2f, 0);
                    tempData.treeResolver = tempData.treeObj.GetComponentInChildren<SpriteResolver>();
                    tempData.animator = tempData.treeObj.GetComponentInChildren<Animator>();

                    //string season = dayCycleHandler.currentSeason.ToString();
                    //string category = tempData.treeResolver.GetCategory();
                    //tempData.treeResolver.SetCategoryAndLabel(category, season);
                    ChangeResolver(ref tempData.treeResolver, dayCycleHandler.currentSeason.ToString());
                }
            }
        }
    }

    private Vector3 RandomXY(SpawnType type)
    {
        float randomX;
        float randomY;

        if (type == SpawnType.UpForest)
        {
            randomX = Random.Range(forestRange[0].position.x, forestRange[1].position.x);
            randomY = Random.Range(forestRange[0].position.y, forestRange[1].position.y);
        }
        else if (type == SpawnType.DownForest)
        {
            randomX = Random.Range(forestRange[2].position.x, forestRange[3].position.x);
            randomY = Random.Range(forestRange[2].position.y, forestRange[3].position.y);
        }
        else if (type == SpawnType.Farm)
        {
            randomX = Random.Range(farmRange[0].position.x, farmRange[1].position.x);
            randomY = Random.Range(farmRange[0].position.y, farmRange[1].position.y);
        }
        else if (type == SpawnType.Quarry)
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
        if (tileManager.croptData.ContainsKey(target) == true)
            return false;
        if (natureData.ContainsKey(target) == true)
            return false;
        if (interactableMap.GetTile(target) == null)
            return false;

        if (tileManager.groundData.ContainsKey(target) == true)
            tileManager.DestroyGroundData(target);

        return true;
    }

    public void RangeSpawnTree(int spawnCount, SpawnType type)
    {
        Vector3Int randomPos;

        for (int i = 0; i < spawnCount;)
        {
            randomPos = tileManager.baseGrid.WorldToCell(RandomXY(type));

            if (SpawnCheck(randomPos) == true)
            {
                TreeData newTree = new();

                newTree.treeObj = RandomTree();

                newTree.isSpawn = true;
                newTree.treeObj.transform.position = (Vector3)randomPos + new Vector3(0.5f, 0.2f, 0);
                newTree.treeResolver = newTree.treeObj.GetComponentInChildren<SpriteResolver>();
                newTree.animator = newTree.treeObj.GetComponentInChildren<Animator>();

                //string season = dayCycleHandler.currentSeason.ToString();
                //string category = newTree.treeResolver.GetCategory();
                //newTree.treeResolver.SetCategoryAndLabel(category, season);
                ChangeResolver(ref newTree.treeResolver, dayCycleHandler.currentSeason.ToString());

                treeData.Add(randomPos, newTree);
                ++i;
            }
        }
    }

    public void RangeSpawnStone(int spawnCount, SpawnType type)
    {
        Vector3Int randomPos;

        for (int i = 0; i < spawnCount;)
        {
            randomPos = tileManager.baseGrid.WorldToCell(RandomXY(type));

            if (SpawnCheck(randomPos) == true)
            {
                StoneData newStone = new();
                newStone.stoneObj = Instantiate(stonePrefab);
                newStone.isSpawn = true;
                newStone.stoneObj.transform.position = (Vector3)randomPos + new Vector3(0.5f, 0.2f, 0);
                newStone.animator = newStone.stoneObj.GetComponentInChildren<Animator>();

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

    public void PickUpNature(Vector3Int target, Vector2 pos)
    {
        Sprite pickUpSprite = natureData[target].natureRenderer.sprite;
        animationController.PickUpAnim(target, pos, pickUpSprite);

        natureData[target].isSpawn = false;
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

        if (stoneData[target].animator.enabled == false)
            stoneData[target].animator.enabled = true;

        stoneData[target].animator.SetTrigger("isFelling");//TODO
        stoneData[target].animator.SetFloat("inputX", direction.x);
        stoneData[target].animator.SetFloat("inputY", direction.y);

        saveTarget = target;
    }

    public void CutTreeTime(bool value)
    {
        if (saveTarget != Vector3Int.zero && treeData.ContainsKey(saveTarget) == true)
        {
            if (treeData[saveTarget].itemDrop == false && treeData[saveTarget].count >= treeData[saveTarget].cutConut)
            {
                //treeData[target].animator.SetTrigger("isFellied");
                treeData[saveTarget].itemDrop = true;

                treeData[saveTarget].animator.enabled = false;
                treeData[saveTarget].animator.runtimeAnimatorController = posAnimator;

                ChangeResolver(ref treeData[saveTarget].treeResolver, "0");
                //string type = treeData[saveTarget].treeResolver.GetCategory();
                //treeData[saveTarget].treeResolver.SetCategoryAndLabel(type, "0");

                //treeData[saveTarget].animator.enabled = true;
                treeData[saveTarget].treeObj.GetComponentInChildren<PolygonCollider2D>().enabled = false;

                Vector3 spawItemPos = (player.transform.position - treeData[saveTarget].treeObj.transform.position).normalized;
                Vector3 dropPos = new();
                if (spawItemPos.x < 0)
                    dropPos.x = 1f;
                else if (spawItemPos.x > 0)
                    dropPos.x = -1f;

                DropItem(treeData[saveTarget].treeObj.transform.position + dropPos, 10, 20001001);
            }
        }
    }

    public void DestroyTree(bool value)
    {
        if (saveTarget != Vector3Int.zero && treeData.ContainsKey(saveTarget) == true)
        {
            if (treeData[saveTarget].count >= treeData[saveTarget].maxCount)
            {
                DropItem(treeData[saveTarget].treeObj.transform.position, 5, 20001001);
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
                DropItem(stoneData[saveTarget].stoneObj.transform.position, 5, 20001002);
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
            go.transform.position = new Vector3(target.x, target.y + 0.5f);
        }
    }

    public void SpriteChange(Season current)
    {
        foreach (var (cell, temp) in treeData)
        {
            if (temp.animator == null)
                continue;
            temp.animator.enabled = false;

            //string category = temp.treeResolver.GetCategory();
            //temp.treeResolver.SetCategoryAndLabel(category, current.ToString());
            ChangeResolver(ref temp.treeResolver, current.ToString());
        }
    }

    private void ChangeResolver(ref SpriteResolver resolver, string current)
    {
        string category = resolver.GetCategory();
        resolver.SetCategoryAndLabel(category, current);
    }
}

