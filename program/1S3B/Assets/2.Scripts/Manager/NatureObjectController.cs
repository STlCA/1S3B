using Constants;
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
using UnityEngine.U2D.Animation;


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
}

public class NatureObjectController : Manager
{
    private Player player;
    private TileManager tileManager;
    private TargetSetting targetSetting;
    private AnimationController animationController;

    [Header("Nature")]
    public GameObject naturePointObject;
    private Transform[] naturePoint;
    public GameObject naturePrefab;
    public SpriteLibraryAsset natureLibrary;

    [Header("Tree")]
    public Transform[] treeRange;
    public GameObject treePointObject;
    private Transform[] treePoint;
    public GameObject tree1Prefab;
    public GameObject tree2Prefab;
    public RuntimeAnimatorController posAnimator;

    [Header("Item")]
    public GameObject dropItemPrefab;

    private Dictionary<Vector3Int, NatureData> natureData = new();
    private Dictionary<Vector3Int, TreeData> treeData = new();

    private Vector3Int saveTarget = new();

    private void Start()
    {
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        animationController = gameManager.AnimationController;
        player = gameManager.Player;

        if (naturePointObject != null)
            naturePoint = naturePointObject.GetComponentsInChildren<Transform>();
        if (treePointObject != null)
            treePoint = treePointObject.GetComponentsInChildren<Transform>();

        animationController.useAnimEnd += DropItemTime;
        animationController.useAnimEnd += DestroyTree;

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
            if (tempdData.isSpawn == false && tileManager.croptData.ContainsKey(cell) == false && treeData.ContainsKey(cell) == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    //Init()으로묶기
                    tempdData.isSpawn = true;
                    tempdData.natureObj = Instantiate(naturePrefab);
                    tempdData.natureObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempdData.natureRenderer = tempdData.natureObj.GetComponent<SpriteRenderer>();
                    tempdData.natureResolver = tempdData.natureObj.GetComponent<SpriteResolver>();

                    string season = "Spring";//계절알아오기

                    IEnumerable<string> names = natureLibrary.GetCategoryLabelNames(season);

                    random = Random.Range(0, names.Count());

                    tempdData.natureResolver.SetCategoryAndLabel(season, random.ToString());
                    //tempData.natureRenderer.sprite = natureLibrary.GetSprite(season, random.ToString());
                }
            }
        }
    }

    public void PointSpawnTree()
    {
        float percentage = 50;
        float randomPoint;
        int random;

        foreach (var (cell, tempData) in treeData)
        {
            if (tempData.isSpawn == false && tileManager.croptData.ContainsKey(cell) == false && natureData.ContainsKey(cell) == false)
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    random = Random.Range(1, 3);

                    if (random == 1)
                        tempData.treeObj = Instantiate(tree1Prefab);
                    else
                        tempData.treeObj = Instantiate(tree2Prefab);

                    tempData.isSpawn = true;
                    //tempData.treeObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempData.treeObj.transform.position = (Vector3)cell + new Vector3(0.5f, 0.2f, 0);
                    tempData.treeResolver = tempData.treeObj.GetComponentInChildren<SpriteResolver>();
                    tempData.animator = tempData.treeObj.GetComponentInChildren<Animator>();

                    string season = "Spring";
                    tempData.treeResolver.SetCategoryAndLabel("Tree", season);
                }
            }
        }
    }

    public void RangeSpawnTree(int spawnCount)
    {
        Vector3Int randomPos;
        int type;

        for (int i = 0; i < spawnCount;)
        {
            float randomX = Random.Range(treeRange[0].position.x, treeRange[1].position.x);
            float randomY = Random.Range(treeRange[0].position.y, treeRange[1].position.y);

            randomPos = tileManager.baseGrid.WorldToCell(new Vector3(randomX, randomY));

            if (treeData.ContainsKey(randomPos) == false && tileManager.croptData.ContainsKey(randomPos) == false && natureData.ContainsKey(randomPos) == false)
            {
                if (tileManager.interactableTileMap.GetTile(randomPos) == null)
                    continue;

                TreeData newTree = new();
                type = Random.Range(1, 3);

                if (type == 1)
                    newTree.treeObj = Instantiate(tree1Prefab);
                else
                    newTree.treeObj = Instantiate(tree2Prefab);

                newTree.isSpawn = true;
                newTree.treeObj.transform.position = (Vector3)randomPos + new Vector3(0.5f, 0.2f, 0);
                newTree.treeResolver = newTree.treeObj.GetComponentInChildren<SpriteResolver>();
                newTree.animator = newTree.treeObj.GetComponentInChildren<Animator>();

                string season = "Spring";
                newTree.treeResolver.SetCategoryAndLabel("Tree", season);

                treeData.Add(randomPos, newTree);
                ++i;
            }
        }
    }

    public bool IsPickUp(Vector3Int target)
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
        treeData[target].animator.SetTrigger("fall");//계절받아오기
        treeData[target].animator.SetFloat("inputX", direction.x);
        treeData[target].animator.SetFloat("inputY", direction.y);
        
        saveTarget = target;
    }

    public void DropItemTime(bool value)
    {
        if (saveTarget != Vector3Int.zero && treeData.ContainsKey(saveTarget) == true)
        {
            if (treeData[saveTarget].itemDrop == false && treeData[saveTarget].count >= treeData[saveTarget].cutConut)
            {
                //treeData[target].animator.SetTrigger("isFellied");
                treeData[saveTarget].itemDrop = true;

                string type = treeData[saveTarget].treeResolver.GetCategory();

                treeData[saveTarget].animator.enabled = false;
                treeData[saveTarget].animator.runtimeAnimatorController = posAnimator;
                treeData[saveTarget].animator.enabled = true;
                treeData[saveTarget].treeResolver.SetCategoryAndLabel(type, "0");
                treeData[saveTarget].treeObj.GetComponentInChildren<PolygonCollider2D>().enabled = false;

                Vector3 spawItemPos = (player.transform.position - treeData[saveTarget].treeObj.transform.position).normalized;
                Vector3 dropPos = new();
                if (spawItemPos.x < 0)
                    dropPos.x = 1f;
                else if (spawItemPos.x > 0)
                    dropPos.x = -1f;

                DropItem(treeData[saveTarget].treeObj.transform.position + dropPos, 10);
            }
        }
    }

    public void DestroyTree(bool value)
    {
        if (saveTarget != Vector3Int.zero && treeData.ContainsKey(saveTarget) == true)
        {
            if (treeData[saveTarget].count >= treeData[saveTarget].maxCount)
            {
                DropItem(treeData[saveTarget].treeObj.transform.position, 5);
                Destroy(treeData[saveTarget].treeObj);
                treeData[saveTarget].isSpawn = false;
                treeData[saveTarget].itemDrop = false;
                treeData[saveTarget].count = 0;
            }
        }
    }

    private void DropItem(Vector3 target, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject go = Instantiate(dropItemPrefab);
            go.transform.position = new Vector3(target.x, target.y + 0.5f);
        }
    }

    public void SpriteChange()//Sprite바꿀때 애니메이터 꺼두고 애니션 실행할때 다시 켜
    {
        foreach (var (cell,temp) in treeData)
        {
            if (temp.animator == null)
                continue;
            temp.animator.enabled = false;
            temp.treeResolver.SetCategoryAndLabel("Tree", "Fall");
        }
    }
}
