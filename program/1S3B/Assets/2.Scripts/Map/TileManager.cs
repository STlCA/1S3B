using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GroundData
{
    public bool isWater;
}

public class CropData
{
    public int cropID;
    public Crop plantCrop;
    public int currentStage;
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
        cropData.plantCrop = DataManager.cropDatabase.GetItemByKey(cropData.cropID);
        cropData.currentStage = 0;
        croptData.Add(target, cropData);

        
        //go.transform.position = 

    }

    public void WaterAt(Vector3Int target)
    {
        if (GameManager.Instance.targetSetting.TargetUI() == false)
            return;

        var tempGroundData = groundData[target];

        tempGroundData.isWater = true;

        waterTilemap.SetTile(target, wateredTile);
    }



    public void Sleep()
    {
        foreach (var (cell, TempgroundData) in groundData)
        {
            TempgroundData.isWater = false;
        }

        waterTilemap.ClearAllTiles();

        foreach(var (cell, tempPlantData)in croptData)
        {
            tempPlantData.plantCrop.DeathTimer -= 1;//하루가 갈수록 -1씩 / 처음에 심을때ㅐ 한 계절인 28에서 지금 날짜 빼기
        }
    }
}
