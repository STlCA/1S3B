using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GroundData
{
    public bool isWater;
}

public class CropData
{
    public GameObject cropObj;
    public SpriteRenderer cropRenderer;
    public int cropID;
    public Crop plantCrop;
    public decimal currentStage;
    public decimal growRatio;
    public int harvest;
}

public class TileManager : MonoBehaviour
{
    [Header("TileMap")]
    public Grid baseGrid;
    public Tilemap backgroundTilemap;
    public Tilemap plantsTilemap;//씨앗뿌릴맵
    public Tilemap waterTilemap;//물뿌릴맵

    [Header("TileCheck")]
    public Tilemap interactableTileMap;//내가 만들어볼용
    public TileBase tilleableTile;//갈수있는땅타일종류(체크용)rule타일이여도되려나//혹은 리스트all써서?

    [Header("TileType")]//tilebase가아니라 rule타일이여도되려나
    public TileBase tilledTile;//간타일
    public TileBase wateredTile;//물뿌린타일

    [Header("Object")]
    public GameObject cropGoPrefabs;

    private Dictionary<Vector3Int, GroundData> groundData = new();//좌표가 키값 GroundData가 value 받아오기
    private Dictionary<Vector3Int, CropData> croptData = new();

    private void Start()
    {
        GameManager.Instance.tileManager = this;
    }

    //샘플
    public bool IsTillableSample(Vector3Int target)//갈수있는땅
    {
        return backgroundTilemap.GetTile(target) == tilleableTile;
    }

    //내가한거
    public bool isInteractable(Vector3Int target)//상호작용할수있는땅
    {
        return interactableTileMap.GetTile(target) != null;
    }
    public bool IsTilled(Vector3Int target)//갈려있는 땅 확인
    {
        return groundData.ContainsKey(target);//키가있다면 갈려있는거
    }
    public bool IsPlantable(Vector3Int target)//갈려있고 씨앗이 심어져있지 않다면
    {
        return IsTilled(target) && !croptData.ContainsKey(target);
    }
    public bool IsHarvest(Vector3Int target)
    {
        return croptData[target].cropRenderer.sprite == croptData[target].plantCrop.SpriteList[croptData[target].plantCrop.AllGrowthStage];
    }



    public void TillAt(Vector3Int target)//밭 가는 작업
    {
        //밭이 갈려있다면 체크 - 장비쪽 메서드에서 갈수있는땅인지 체크 거기서 tillat부르기
        if (GameManager.Instance.targetSetting.TargetUI() == false)
            return;

        backgroundTilemap.SetTile(target, tilledTile);
        groundData.Add(target, new GroundData());//좌표에 정보만넣어주는거지 타일에 무언가 직접하는건 아님
    }

    public void PlantAt(Vector3Int target)
    {
        if (GameManager.Instance.targetSetting.TargetUI() == false)
            return;

        CropData cropData = new CropData();
        cropData.cropID = Random.Range(1001, 1004);//임시//임시로 랜덤할까
        cropData.plantCrop = GameManager.Instance.dataManager.cropDatabase.GetItemByKey(cropData.cropID);
        cropData.currentStage = 0;
        cropData.growRatio = cropData.plantCrop.AllGrowthStage / (decimal)cropData.plantCrop.GrowthTime;
        cropData.cropObj = Instantiate(cropGoPrefabs);
        cropData.cropObj.transform.position = baseGrid.GetCellCenterWorld(target);
        cropData.cropRenderer = cropData.cropObj.GetComponentInChildren<SpriteRenderer>();
        cropData.cropRenderer.sprite = cropData.plantCrop.SpriteList[0];
        //cropData.plantCrop.DeathTimer = 


        //+모지리세션

        croptData.Add(target, cropData);
    }

    public void WaterAt(Vector3Int target)
    {
        if (GameManager.Instance.targetSetting.TargetUI() == false)
            return;

        // 물주려고 봤더니? 플레이어 인풋에서 체크? 최대면 수확
        // 수확 후 단계 체크하고 그쪽 스프라이트로 변경
        // 최대 인덱스를 넘어가지않게

        var tempGroundData = groundData[target];

        tempGroundData.isWater = true;

        waterTilemap.SetTile(target, wateredTile);
    }

    public void Harvest(Vector3Int target)
    {
        if (croptData[target].plantCrop.StageAfterHarvest == 0)//바로삭제
        {
            //인벤넣기
            Destroy(croptData[target].cropObj);
            croptData.Remove(target);
        }
        else//여러번수확
        {
            //인벤넣기
            croptData[target].harvest++;
            croptData[target].currentStage = croptData[target].plantCrop.StageAfterHarvest;
            croptData[target].cropRenderer.sprite = croptData[target].plantCrop.SpriteList[croptData[target].plantCrop.StageAfterHarvest];
        }
    }


    public void Sleep()
    {
        foreach (var (cell, tempPlantData) in croptData)
        {
            tempPlantData.plantCrop.DeathTimer -= 1;//하루가 갈수록 -1씩 / 처음에 심을때ㅐ 한 계절인 28에서 지금 날짜 빼기

            if (groundData[cell].isWater == true)
            {
                if (tempPlantData.harvest > 0)
                    tempPlantData.growRatio = (tempPlantData.plantCrop.AllGrowthStage - tempPlantData.plantCrop.StageAfterHarvest) / (decimal)tempPlantData.plantCrop.ReGrowthTime;

                tempPlantData.currentStage += tempPlantData.growRatio;

                int temp = (int)(tempPlantData.currentStage);

                if(temp >= tempPlantData.plantCrop.AllGrowthStage)
                {
                    tempPlantData.cropRenderer.sprite = tempPlantData.plantCrop.SpriteList[tempPlantData.plantCrop.AllGrowthStage];
                    tempPlantData.cropObj.tag = "Harvest";
                }
                else
                    tempPlantData.cropRenderer.sprite = tempPlantData.plantCrop.SpriteList[temp];

                // 최대 인덱스를 넘어가지않게
                //마지막인덱스때 콜리더생성 or 타일맵에 투명타일생성 or tag생성(두둥)
                // 총 자라는 시간 % 단계 스프라이트 = 비율 맞춰서 비율 int로 변경한만큼 스프라이트변경
            }
        }

        foreach (var (cell, TempgroundData) in groundData)
        {
            TempgroundData.isWater = false;
        }

        waterTilemap.ClearAllTiles();

    }
}
