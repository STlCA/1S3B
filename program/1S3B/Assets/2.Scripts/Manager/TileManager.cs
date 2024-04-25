using Constants;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct SaveTileData
{
    public List<Vector3Int> GroundCellPos;
    public List<GroundData> GroundDatas;

    public List<Vector3Int> CropDataCellPos;
    public List<SaveCropData> CropDatas;
}

[System.Serializable]
public struct SaveCropData
{
    public int cropID;
    public decimal currentStage;
    public decimal growRatio;
    public int harvest;
    public int deathTimer;
}

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
        cropID = plantCrop.ID;

        if (isWater == true)
            cropRenderer.color = Color.gray;
    }

    public void Save(ref SaveCropData data)
    {
        data.cropID = cropID;
        data.currentStage = currentStage;
        data.growRatio = growRatio;
        data.harvest = harvest;
        data.deathTimer = deathTimer;
    }

    public void Load(SaveCropData data, GameObject go, CropDatabase cropDatabase,Sprite deathCrop)
    {
        cropID = data.cropID;
        currentStage = data.currentStage;
        growRatio = data.growRatio;
        harvest = data.harvest;
        deathTimer = data.deathTimer;

        plantCrop = cropDatabase.GetItemByKey(cropID);
        cropObj = go;
        cropRenderer = cropObj.GetComponentInChildren<SpriteRenderer>();
        cropRenderer.sprite = plantCrop.SpriteList[(int)currentStage];
        cropRenderer.sortingOrder = (int)(cropObj.transform.position.y * 1000 * -1);

        if (currentStage < 1)
            cropRenderer.sortingLayerName = "Seed";
        else if (currentStage == -1)
            cropRenderer.sprite = deathCrop;
        else if (currentStage >= plantCrop.AllGrowthStage)
            cropObj.tag = "Harvest";
        else
            cropObj.tag = "Crop";
    }
}

public class TileManager : Manager
{
    private Player player;
    private Inventory inventory;
    private AnimationController animationController;
    private TargetSetting targetSetting;
    private DayCycleHandler dayCycleHandler;
    private WeatherSystem weatherSystem;
    private SoundSystemManager soundManager;

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
    public Dictionary<Vector3Int, CropData> cropData { get; private set; } = new();

    private CropDatabase cropDatabase;
    private ItemDatabase itemDatabase;
    private bool isRain = false;

    
    public override void Init(GameManager gm)
    {
        base.Init(gm);

        animationController = gameManager.AnimationController;
        targetSetting = gameManager.TargetSetting;
        dayCycleHandler = gameManager.DayCycleHandler;
        weatherSystem = gameManager.WeatherSystem;
        soundManager = gameManager.SoundManager;
        player = gameManager.Player;
        inventory = player.Inventory;

        cropDatabase = gameManager.DataManager.cropDatabase;
        itemDatabase = gameManager.DataManager.itemDatabase;

        weatherSystem.IsRainAction += IsRain;
    }

    //샘플
    public bool IsTillableSample(Vector3Int target)//갈수있는땅
    {
        return backgroundTilemap.GetTile(target) == tilleableTile;
    }

    //내가한거
    public bool IsInteractable(Vector3Int target)//상호작용할수있는땅
    {
        return interactableTileMap.GetTile(target) != null;
    }
    public bool IsTilled(Vector3Int target)//갈려있는 땅 확인
    {
        return groundData.ContainsKey(target);//키가있다면 갈려있는거
    }
    public bool IsPlantable(Vector3Int target)//갈려있고 씨앗이 심어져있지 않다면
    {
        return IsTilled(target) && !cropData.ContainsKey(target);
    }
    public bool IsPlant(Vector3Int target)
    {
        return cropData.ContainsKey(target) == true && IsHarvest(target) == false;
    }
    public bool IsHarvest(Vector3Int target)
    {
        return cropData.ContainsKey(target) == true && cropData[target].cropRenderer.sprite == cropData[target].plantCrop.SpriteList[cropData[target].plantCrop.AllGrowthStage];
    }

    public void TillAt(Vector3Int target)//밭 가는 작업
    {
        soundManager.PlayerAudioClipPlay((int)PlayerAudioClip.Hoe);

        //밭이 갈려있다면 체크 - 장비쪽 메서드에서 갈수있는땅인지 체크 거기서 tillat부르기
        if (targetSetting.TargetUI() == false)
            return;

        tilledTilemap.SetTile(target, tilledTile);

        groundData.Add(target, new GroundData());//좌표에 정보만넣어주는거지 타일에 무언가 직접하는건 아님

        if (isRain == true)
            WaterAt(target, true);

    }

    public void PlantAt(Vector3Int target, Item item)
    {
        if (targetSetting.TargetUI() == false)
            return;

        if (item.quantity <= 0)
            return;

        soundManager.PlayerAudioClipPlay((int)PlayerAudioClip.Seed);

        player.selectItem.quantity--;
        inventory.UseRefresh(player.selectItem);

        CropData tempcropData = new CropData();

        //임시 
        //int[] arr = { 1, 2, 3, 4, 5, 6, 7, 101, 102, 103, 104 };
        //int index = Random.Range(0, 11);
        //int cropID = arr[index];
        int cropID = item.ItemInfo.CropID;

        GameObject go = Instantiate(cropGoPrefabs);
        go.transform.position = baseGrid.GetCellCenterWorld(target);

        bool isWater = groundData[target].isWater;
        int currentDay = dayCycleHandler.currentDay;

        tempcropData.Init(cropID, cropDatabase, go, isWater, currentDay);

        cropData.Add(target, tempcropData);
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

        if (cropData.ContainsKey(target) && cropData[target].cropRenderer.sortingLayerName == "Seed")
            cropData[target].cropRenderer.color = Color.gray;

        if (rain == false && cropData.ContainsKey(target) == true)
            cropData[target].cropObj.GetComponent<ShapeCrop>().ShapeAnimation();
    }

    public void Harvest(Vector3Int target, Vector2 pos)
    {
        if (targetSetting.TargetUI() == false)
            return;

        soundManager.PlayerAudioClipPlay((int)PlayerAudioClip.PickUp);

        Sprite pickUpSprite = cropData[target].plantCrop.SpriteList[cropData[target].plantCrop.SpriteList.Count - 1];
        animationController.PickUpAnim(target, pos, pickUpSprite);

        if (cropData[target].plantCrop.StageAfterHarvest == 0)//바로삭제
        {
            HarvestItem(target);

            DestroyCropData(target);

        }
        else//여러번수확
        {
            HarvestItem(target);

            cropData[target].harvest++;
            cropData[target].currentStage = cropData[target].plantCrop.StageAfterHarvest;
            cropData[target].cropRenderer.sprite = cropData[target].plantCrop.SpriteList[cropData[target].plantCrop.StageAfterHarvest];
            cropData[target].cropObj.tag = "Crop";
        }
    }
    public void Sleep()
    {
        foreach (var (cell, tempPlantData) in cropData)
        {
            tempPlantData.deathTimer--; //하루가 갈수록 -1씩 / 처음에 심을때ㅐ 한 계절인 28에서 지금 날짜 빼기

            if (tempPlantData.deathTimer <= 0)
            {
                tempPlantData.currentStage = -1;
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

    public void DestroyCropData(Vector3Int target)
    {
        Destroy(cropData[target].cropObj);
        cropData.Remove(target);
    }

    public void DestroyGroundData(Vector3Int target)
    {
        tilledTilemap.SetTile(target, null);
        waterTilemap.SetTile(target, null);
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

    private void HarvestItem(Vector3Int target)
    {
        ItemInfo itemInfo = itemDatabase.GetItemByCropID(cropData[target].cropID);
        Item item = new Item();
        item.ItemInfo = itemInfo;
        inventory.AddItem(item);
    }

    //============================================================Save
    public void Save(ref SaveTileData data)
    {
        data.GroundCellPos = new();
        data.GroundDatas = new();

        foreach (var _groundData in groundData)
        {
            data.GroundCellPos.Add(_groundData.Key);
            data.GroundDatas.Add(_groundData.Value);
        }

        data.CropDataCellPos = new();
        data.CropDatas = new();

        foreach (var _cropData in cropData)
        {
            data.CropDataCellPos.Add(_cropData.Key);

            SaveCropData saveData = new();
            _cropData.Value.Save(ref saveData);
            data.CropDatas.Add(saveData);
        }
    }

    public void Load(SaveTileData data)
    {
        groundData = new();

        for (int i = 0; i < data.GroundDatas.Count; ++i)
        {
            groundData.Add(data.GroundCellPos[i], data.GroundDatas[i]);
            tilledTilemap.SetTile(data.GroundCellPos[i], tilledTile);
        }

        cropData = new();
        for (int i = 0; i < data.CropDatas.Count; ++i)
        {
            GameObject go = Instantiate(cropGoPrefabs);
            go.transform.position = baseGrid.GetCellCenterWorld(data.CropDataCellPos[i]);

            CropData newData = new CropData();
            newData.Load(data.CropDatas[i],go,cropDatabase,deathCrop);

            cropData.Add(data.CropDataCellPos[i], newData);
        }

        if(isRain == true)
            RainWatering();
    }
}
