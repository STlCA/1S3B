using Constants;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditor.ShaderGraph;
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
    private TileManager tileManager;
    private TargetSetting targetSetting;
    private AnimationController animationController;

    [Header("Nature")]
    public GameObject naturePointObject;
    private Transform[] naturePoint;
    public GameObject naturePrefab;
    public SpriteLibraryAsset natureLibrary;

    [Header("Tree")]
    public GameObject treePointObject;
    private Transform[] treePoint;
    public GameObject tree1Prefab;
    public GameObject tree2Prefab;
    public GameObject dropItemPrefab;

    private Dictionary<Vector3Int, NatureData> natureData = new();
    private Dictionary<Vector3Int, TreeData> treeData = new();

    private void Start()
    {
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        animationController = gameManager.AnimationController;

        if (naturePointObject != null)
            naturePoint = naturePointObject.GetComponentsInChildren<Transform>();
        if (treePointObject != null)
            treePoint = treePointObject.GetComponentsInChildren<Transform>();

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

    public void SpawnTree()
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
                    tempData.treeObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempData.treeResolver = tempData.treeObj.GetComponent<SpriteResolver>();
                    tempData.animator = tempData.treeObj.GetComponent<Animator>();
                                        
                    string season = "Spring";
                    tempData.treeResolver.SetCategoryAndLabel("Tree", season);                    
                }
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
        Vector3 direction = treeData[target].treeObj.transform.position - GameManager.Instance.Player.transform.position;
        treeData[target].animator.SetTrigger("isFelling");
        treeData[target].animator.SetTrigger("spring");//
        treeData[target].animator.SetFloat("inputX", direction.x);
        treeData[target].animator.SetFloat("inputY", direction.y);


        if (treeData[target].count >= treeData[target].maxCount)
        {
            DropItem(treeData[target].treeObj.transform.position, 5);
            Destroy(treeData[target].treeObj);
            treeData[target].isSpawn = false;
            treeData[target].itemDrop = false;
            treeData[target].count = 0;
        }
        else if (treeData[target].itemDrop == false && treeData[target].count >= treeData[target].cutConut)
        {
            //treeData[target].animator.SetTrigger("isFellied");
            treeData[target].itemDrop = true;

            string type = treeData[target].treeResolver.GetCategory();

            treeData[target].treeResolver.SetCategoryAndLabel(type, "0");
            treeData[target].animator.enabled = false;

            Vector3 spawItemPos = (GameManager.Instance.Player.transform.position - treeData[target].treeObj.transform.position).normalized;
            Vector3 dropPos = new();
            if (spawItemPos.x > 0)
                dropPos.x = 2f;
            else if (spawItemPos.x < 0)
                dropPos.x = -2f;

            DropItem(treeData[target].treeObj.transform.position + dropPos, 10);
        }

    }

    private void DropItem(Vector3 target, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            GameObject go = Instantiate(dropItemPrefab);
            go.transform.position = new Vector3(target.x, target.y + 0.5f);//리지드달기
        }
    }
}
