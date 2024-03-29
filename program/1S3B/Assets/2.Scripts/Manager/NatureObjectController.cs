using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.U2D.Animation;
public class TreeData
{
    public GameObject treeObj;

    public SpriteRenderer treeRenderer;
    public SpriteLibrary treeLibrayry;
    public int maxConut = 10;
    public int count = 0;
}

public class NatureData
{

    public SpriteLibrary natureLibrary;
    public SpriteResolver natureResolver;


    public GameObject natureObj;
    public SpriteRenderer natureRenderer;
    public bool isSpawn = false;
    
}

public class NatureObjectController : Manager
{
    private TileManager tileManager;
    private TargetSetting targetSetting;

    [Header("Nature")]
    public GameObject naturePointObject;
    private GameObject[] naturePoint;
    public GameObject naturePrefab;


    private Dictionary<Vector3Int, NatureData> natureData = new();
    private Dictionary<Vector3Int, TreeData> treeData = new();

    private void Start()
    {
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;

        if(naturePointObject!=null)
            naturePoint = naturePointObject.GetComponentsInChildren<GameObject>();
    }

    public override void Init(GameManager gm)
    {
        base.Init(gm);

        if(naturePoint!=null)
        {
            foreach (GameObject go in naturePoint)
            {
                Vector3Int goCellPos = tileManager.baseGrid.WorldToCell(go.transform.position);

                NatureData tempData = new NatureData();

                natureData.Add(goCellPos, tempData);
            }
        }
    }

    private void SpawnNature()
    {
        float percentage = 50;
        float randomPoint;
        //int random;

        foreach (var (cell, tempdData) in natureData)
        {            
            if(tempdData.isSpawn == false)
            {
                randomPoint = Random.value * percentage;
                if (randomPoint < percentage)
                {
                    //Init()으로묶기
                    tempdData.isSpawn = true;
                    tempdData.natureObj = Instantiate(naturePrefab);
                    tempdData.natureObj.transform.position = tileManager.baseGrid.GetCellCenterWorld(cell);
                    tempdData.natureObj.tag = "Harvest";

                    string str = "spring";//tempdData.natureResolver.GetCategory();

                    tempdData.natureLibrary.spriteLibraryAsset.GetCategoryLabelNames(str);

                    //tempdData.natureResolver.SetCategoryAndLabel("Spring", "0");
                    //
                    //tempdData.natureLibrary.GetSprite(Spring)
                    //
                    //random = Random.Range(0, tempdData.natureResolver.GetCategory())
                    //
                    //
                    //
                    //tempdData.natureRenderer.sprite = tempdData.natureLibrary.GetSprite()

                }                    
            }                
        }









        /*for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
    출처: https://geojun.tistory.com/83 [Jungle(정글):티스토리]



        int checkIndex = 0;
        float total = 0;



        //float[] itemPercent = new float[spawnItem.Length];
        //for (int i = 0; i < spawnItem.Length; i++)
        //{
        //    float percent = spawnItem[i].GetComponent<Item>().Percent;
        //    itemPercent[i] = percent;
        //    total += percent;
        //}



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

        if (!spawnItem[dropIndex].GetComponent<Item>().Useitem)
        {
            spawnItem[dropIndex].SetActive(true);
            spawnItem[dropIndex].transform.position = gameObject.transform.position;
        }*/
    }



    public bool IsLogging(Vector3Int target)
    {
        return treeData.ContainsKey(target);
    }

    public void LoggingAt(Vector3Int target)
    {
        if (targetSetting == null)
            targetSetting = GameManager.Instance.TargetSetting;

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
