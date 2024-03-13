using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GroundData
{
    public bool isWater;
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


    private void Awake()
    {
    }
    private void Start()
    {
        TempGameManager.instance.tileManager = this;
    }
    private void Update()
    {

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

    public void TillAt(Vector3Int target)//밭 가는 작업
    {
        //밭이 갈려있다면 체크 - 장비쪽 메서드에서 갈수있는땅인지 체크 거기서 tillat부르기
        if (TempGameManager.instance.targetSetting.TargetUI() == false)
            return;

        backgroundTilemap.SetTile(target, tilledTile);
        groundData.Add(target, new GroundData());//좌표에 정보만넣어주는거지 타일에 무언가 직접하는건 아님
    }

    public void WaterAt(Vector3Int target)
    {
        if (TempGameManager.instance.targetSetting.TargetUI() == false)
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
    }



}
