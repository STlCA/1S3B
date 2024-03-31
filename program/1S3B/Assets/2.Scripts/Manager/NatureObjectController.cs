using Constants;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TreeEditor;
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
    public SpriteRenderer treeRenderer;
    public SpriteResolver treeResolver;
    public Animator animator;
    public float count = 0;
    public int cutConut = 10;
    public int maxCount = 15;
    public bool isSpawn = false;
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
    public GameObject treePrefab;
    public SpriteLibraryAsset treeLibrary;
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
            if (tempdData.isSpawn == false && tileManager.croptData.ContainsKey(cell) == false && treeData.ContainsKey(cell))
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
            if (tempData.isSpawn == false && tileManager.croptData.ContainsKey(cell) == false && natureData.ContainsKey(cell))
            {
                randomPoint = Random.Range(0, 101);
                if (randomPoint < percentage)
                {
                    //Init()으로묶기
                    tempData.isSpawn = true;
                    tempData.treeObj = Instantiate(naturePrefab);
                    tempData.treeObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempData.treeRenderer = tempData.treeObj.GetComponent<SpriteRenderer>();
                    tempData.treeResolver = tempData.treeObj.GetComponent<SpriteResolver>();
                    tempData.animator = tempData.treeObj.GetComponent<Animator>();

                    string season = "Spring";//계절알아오기

                    IEnumerable<string> names = treeLibrary.GetCategoryLabelNames(season);

                    random = Random.Range(0, names.Count());

                    tempData.treeResolver.SetCategoryAndLabel(season, random.ToString());
                    //tempData.natureRenderer.sprite = natureLibrary.GetSprite(season, random.ToString());
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
        return treeData.ContainsKey(target);
    }

    public void Felling(Vector3Int target)
    {
        if (targetSetting.PlayerBoundCheck() == false)
            return;

        treeData[target].count--;
        treeData[target].animator.SetTrigger("isFelling");

        if (treeData[target].count >= treeData[target].cutConut)
        {
            treeData[target].animator.SetTrigger("isFellied");

            string treeSet = "First";

            treeData[target].treeResolver.SetCategoryAndLabel(treeSet, "0");

            Vector3 spawItemPos = (GameManager.Instance.Player.transform.position - treeData[target].treeObj.transform.position).normalized;

            DropItem(treeData[target].treeObj.transform.position + spawItemPos, 10);
        }
        if (treeData[target].count >= treeData[target].maxCount)
        {
            DropItem(treeData[target].treeObj.transform.position, 5);
            Destroy(treeData[target].treeObj);
            treeData.Remove(target);
        }
    }

    private void DropItem(Vector3 target, int count)
    {
        for ( int i = 0; i < count; ++i)
        {
            GameObject go = Instantiate(dropItemPrefab);
            go.transform.position = new Vector3(target.x, target.y + 0.5f);//리지드달기
        }
    }
}
