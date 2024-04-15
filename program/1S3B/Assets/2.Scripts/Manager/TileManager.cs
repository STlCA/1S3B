using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

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
    public int deathTimer;


    public void Init(int id, CropDatabase cropDatabase, GameObject go, bool isWater, int currentDay)
    {
        plantCrop = cropDatabase.GetItemByKey(id);
        currentStage = 0;
        growRatio = plantCrop.AllGrowthStage / (decimal)plantCrop.GrowthTime;
        cropObj = go;
        cropRenderer = cropObj.GetComponentInChildren<SpriteRenderer>();
        cropRenderer.sprite = plantCrop.SpriteList[0];
        cropRenderer.sortingOrder = (int)(cropObj.transform.position.y * 1000 * -1);
        cropRenderer.sortingLayerName = "Seed";
        deathTimer = plantCrop.DeathTimer - currentDay % 28;

        if (isWater == true)
            cropRenderer.color = Color.gray;
    }
}

public class TileManager : Manager
{
    private AnimationController animationController;
    private TargetSetting targetSetting;
    private DayCycleHandler dayCycleHandler;

    [Header("TileMap")]
    public Grid baseGrid;
    public Tilemap backgroundTilemap;
    public Tilemap tilledTilemap;
    public Tilemap plantsTilemap;//씨앗뿌릴맵 // 나중에 씨앗만 물아래에 깔리게
    public Tilemap waterTilemap;//물뿌릴맵

    [Header("TileCheck")]
    public Tilemap interactableTileMap;//내가 만들어볼용
    public TileBase tilleableTile;//갈수있는땅타일종류(체크용)rule타일이여도되려나//혹은 리스트all써서?

    [Header("TileType")]//tilebase가아니라 rule타일이여도되려나
    public TileBase tilledTile;//간타일
    public TileBase wateredTile;//물뿌린타일

    [Header("Object")]
    public GameObject cropGoPrefabs;
    public Sprite deathCrop;

    public Dictionary<Vector3Int, GroundData> groundData { get; private set; } = new();//좌표가 키값 GroundData가 value 받아오기
    public Dictionary<Vector3Int, CropData> croptData { get; private set; } = new();

    private CropDatabase cropDatabase;
    private bool isRain = false;

    private void Start()
    {
        animationController = gameManager.AnimationController;
        targetSetting = gameManager.TargetSetting;
        dayCycleHandler = gameManager.DayCycleHandler;

        cropDatabase = gameManager.DataManager.cropDatabase;
    }

    //샘플
    public bool IsTillableSample(Vector3Int target)//갈수있는땅
    {
        return backgroundTilemap.GetTile(target) == tilleableTile;
    }

    //내가한거
    public bool IisInteractable(Vector3Int target)//상호작용할수있는땅
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
        if (targetSetting.TargetUI() == false)
            return;

        tilledTilemap.SetTile(target, tilledTile);

        groundData.Add(target, new GroundData());//좌표에 정보만넣어주는거지 타일에 무언가 직접하는건 아님

        if (isRain == true)
            WaterAt(target, true);

    }

    public void PlantAt(Vector3Int target)
    {
        if (targetSetting.TargetUI() == false)
            return;

        CropData tempcropData = new CropData();

        //임시 
        int[] arr = { 1, 2, 3, 4, 5, 6, 7, 101, 102, 103, 104 };
        int index = Random.Range(0, 11);
        int cropID = arr[index];

        GameObject go = Instantiate(cropGoPrefabs);
        go.transform.position = baseGrid.GetCellCenterWorld(target);

        bool isWater = groundData[target].isWater;
        int currentDay = dayCycleHandler.currentDay;

        tempcropData.Init(cropID, cropDatabase, go, isWater, currentDay);

        croptData.Add(target, tempcropData);
    }

    public void WaterAt(Vector3Int target, bool rain = false)
    {
        if (rain == false && targetSetting.TargetUI() == false)
            return;

        // 물주려고 봤더니? 플레이어 인풋에서 체크? 최대면 수확
        // 수확 후 단계 체크하고 그쪽 스프라이트로 변경
        // 최대 인덱스를 넘어가지않게

        groundData[target].isWater = true;
        waterTilemap.SetTile(target, wateredTile);

        if (croptData.ContainsKey(target) && croptData[target].cropRenderer.sortingLayerName == "Seed")
            croptData[target].cropRenderer.color = Color.gray;

        if (rain == false)
            croptData[target].cropObj.GetComponent<ShapeCrop>().ShapeAnimation();

    }

    public void Harvest(Vector3Int target, Vector2 pos)
    {
        if (targetSetting.TargetUI() == false)
            return;

        Sprite pickUpSprite = croptData[target].plantCrop.SpriteList[croptData[target].plantCrop.SpriteList.Count - 2];
        animationController.PickUpAnim(target, pos, pickUpSprite);

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
            croptData[target].cropObj.tag = "Crop";
        }
    }

    public void Sleep()
    {
        foreach (var (cell, tempPlantData) in croptData)
        {
            tempPlantData.deathTimer--; //하루가 갈수록 -1씩 / 처음에 심을때ㅐ 한 계절인 28에서 지금 날짜 빼기

            if (tempPlantData.deathTimer <= 0)
            {
                tempPlantData.cropRenderer.sprite = deathCrop;
                tempPlantData.cropRenderer.sortingLayerName = "Default";
                tempPlantData.cropRenderer.color = Color.white;
            }
            else if (groundData[cell].isWater == true)
            {
                if (tempPlantData.harvest > 0)
                    tempPlantData.growRatio = (tempPlantData.plantCrop.AllGrowthStage - tempPlantData.plantCrop.StageAfterHarvest) / (decimal)tempPlantData.plantCrop.ReGrowthTime;

                tempPlantData.currentStage += tempPlantData.growRatio;

                int temp = (int)(tempPlantData.currentStage);

                if (temp >= 1)
                    tempPlantData.cropObj.tag = "Crop";

                if (temp >= tempPlantData.plantCrop.AllGrowthStage)
                {
                    tempPlantData.cropRenderer.sprite = tempPlantData.plantCrop.SpriteList[tempPlantData.plantCrop.AllGrowthStage];
                    tempPlantData.cropObj.tag = "Harvest";
                }
                else
                    tempPlantData.cropRenderer.sprite = tempPlantData.plantCrop.SpriteList[temp];

                if (temp != 0)
                    tempPlantData.cropRenderer.sortingLayerName = "Default";

                tempPlantData.cropRenderer.color = Color.white;

                // 최대 인덱스를 넘어가지않게
                // 마지막인덱스때 콜리더생성 or 타일맵에 투명타일생성 or tag생성(두둥)
                // 총 자라는 시간 % 단계 스프라이트 = 비율 맞춰서 비율 int로 변경한만큼 스프라이트변경
            }


        }

        foreach (var (cell, TempgroundData) in groundData)
        {
            TempgroundData.isWater = false;
        }

        waterTilemap.ClearAllTiles();

    }

    public void DestroyGroundData(Vector3Int target)
    {
        groundData.Remove(target);
    }

    public void IsRain(bool isRain)
    {
        this.isRain = isRain;

        if (isRain == true)
            RainWatering();
    }

    public void RainWatering()
    {
        foreach (var (cell, ground) in groundData)
        {
            WaterAt(cell, true);
        }

        //땅 새로팔때도 지금은 데이터만 바꿨으니 물타일 넣어야하고 새로팔때도 넣어야하고 날씨가 끝나면 false로 바꿔야하고
    }
}
