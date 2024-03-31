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
public class TreeData
{
    public GameObject treeObj;
    public SpriteRenderer treeRenderer;
    public SpriteLibrary treeLibrayry;
    public int maxConut = 10;
    public int count = 0;
}

[System.Serializable]
public class NatureData
{
    public GameObject natureObj;
    public SpriteRenderer natureRenderer;
    public SpriteResolver natureResolver;
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

    [SerializeField]
    private Dictionary<Vector3Int, NatureData> natureData = new();
    private Dictionary<Vector3Int, TreeData> treeData = new();

    private void Start()
    {
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        animationController = gameManager.AnimationController;

        if (naturePointObject != null)
            naturePoint = naturePointObject.GetComponentsInChildren<Transform>();

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
    }

    public void SpawnNature()
    {
        float percentage = 50;
        float randomPoint;
        int random;

        foreach (var (cell, tempdData) in natureData)
        {
            if (tempdData.isSpawn == false)
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
                    //tempdData.natureRenderer.sprite = natureLibrary.GetSprite(season, random.ToString());
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

        if (treeData[target].count == treeData[target].maxConut)
        {
            //애니메이션 실행
            treeData[target].treeRenderer.sprite = treeData[target].treeLibrayry.GetSprite("tree", "0");
        }
    }
}
