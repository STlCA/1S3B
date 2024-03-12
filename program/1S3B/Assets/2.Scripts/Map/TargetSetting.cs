using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.U2D.Animation;
using UnityEngine.UIElements;
using System.Linq;
using System.Reflection.Emit;

[System.Serializable]
public class Plants
{
    public GameObject go;
    public SpriteRenderer spriteRenderer;
    public SpriteResolver spriteResolver;
}

public class TargetSetting : MonoBehaviour
{
    [Header("PlayerObject")]
    public GameObject playerObj;
    public GameObject targetSprite;

    [Header("TileMap")]
    public Tilemap interactableMap;//범위확인땅
    public Tilemap seedMap;//갈수있는땅
    public Tilemap waterMap;
    public Tile interactableTile;
    public Tile seedTile;
    public Tile waterTile;

    private Vector3Int playerCellPosition;
    private Vector3Int selectCellPosition;

    public GameObject Temp;
    public SpriteLibraryAsset sprite;
    private SpriteRenderer plants;

    private SpriteResolver resolve;

    private bool isWater = false;

    private List<Plants> seeds = new();

    private void Update()
    {
        if (interactableMap != null)
            playerCellPosition = interactableMap.WorldToCell(playerObj.transform.position);
    }

    public void SetCellPosition(Vector3 value)
    {
        selectCellPosition = interactableMap.WorldToCell(value);

        if (interactableMap.GetTile(selectCellPosition) == null)
            return;

        if (PlayerBoundCheck() == true)//움직이면 안보이는기능추가해야할듯
        {
            targetSprite.SetActive(true);
            TargetUI();
        }
        else
            targetSprite.SetActive(false);
    }

    private bool PlayerBoundCheck()
    {
        Vector3 bound = playerCellPosition - selectCellPosition;

        if (-1f <= bound.x && bound.x <= 1f && -1f <= bound.y && bound.y <= 1f)
            return true;
        else
            return false;
    }

    private void TargetUI()
    {
        gameObject.transform.position = selectCellPosition + new Vector3(0.5f, 0.5f);
    }


    public void TileCheck()
    {
        

        if (interactableMap == null)
            return;
        
        if (PlayerBoundCheck() == false)
            return;

        //if (interactableMap.GetTile(selectCellPosition) == interactableMap && backgroundMap.GetTile(selectCellPosition) != interactableTile)//안갈린 갈수있는땅
        //{
        //    Debug.Log("맞다야");
        //    backgroundMap.SetTile(selectCellPosition, interactableTile);
        //
        //}
        //else if (backgroundMap.GetTile(selectCellPosition) == interactableTile)//이미갈린땅
        //{
        //
        //}

        //땅과 작용할수있는거 괭이 물뿌리개 곡괭이 씨앗

        if (interactableMap.GetTile(selectCellPosition) != null && seedMap.GetTile(selectCellPosition) == null)//안갈린 갈수있는땅 // 괭이
        {
            Debug.Log("갈수있는땅맞다야");
            // 괭이인지 확인
            seedMap.SetTile(selectCellPosition, interactableTile);

        }
        else if (seedMap.GetTile(selectCellPosition) != null && seedMap.GetTile(selectCellPosition) != seedTile)//이미갈린땅 // 물뿌리개 곡괭이 씨앗
        {
            Debug.Log("씨앗심을수있는땅");

            //Plants plant = new();
            //plant.go = Instantiate(Temp);
            //plant.go.transform.position = selectCellPosition + new Vector3(0.5f, 0.5f);
            //plant.spriteRenderer = plant.go.GetComponentInChildren<SpriteRenderer>();
            //plant.spriteResolver = plant.go.GetComponentInChildren<SpriteResolver>();
            //plant.spriteRenderer.sprite = sprite.GetSprite("IDN", "1");
            //seeds.Add(plant);

            GameObject go = Instantiate(Temp);

            go.transform.position = selectCellPosition + new Vector3(0.5f,0.5f);
            plants = go.GetComponentInChildren<SpriteRenderer>();
            plants.sprite = sprite.GetSprite("IDN", "1");

            seedMap.SetTile(selectCellPosition, seedTile);

            resolve = go.GetComponentInChildren<SpriteResolver>();


            //씨앗심은땅의 spriteLibrary, resolver, spriterenderer, position을 관리해야함
            

            //Instantiate(sprite.GetSprite("temp", "0"));

            //resolve -> sprite.SetCategoryAndLabel("temp", 1);
            //씨앗들고있는지 체크
            //무슨씨앗인지체크
            //오브젝트 인스탄티에트?

            //카테고리 이름 체크
            //string[] labels;
            //
            //IEnumerable<string> str = sprite.GetCategoryLabelNames("IDN");
            //labels = new string[str.Count()];
            //int index = 0;
            //foreach (string name in str)
            //{
            //    Debug.Log(name);
            //    labels[index] = name;
            //    index++;
            //}
            //카테고리 이름체크

        }
        else if (seedMap.GetTile(selectCellPosition) == seedTile)
        {
            //대충 물뿌리개들었으면

            //seedMap.SetTile(selectCellPosition, waterTile);
            waterMap.SetTile(selectCellPosition, waterTile);

            isWater = true;

            Debug.Log("씨앗뿌려진 땅 물주기");
        }

    }

    public void OneDay()
    {
        if (isWater == false)
            return;

        IEnumerable<string> str = sprite.GetCategoryLabelNames("IDN");
        string[] labels = new string[str.Count()];
        int index = 0;
        string label;
        int ind = 0;
        foreach (string name in str)
        {
            Debug.Log(name);
            labels[index] = name;
        
            //if(name == seeds.All<)
            if (name == resolve.GetLabel())
                ind = index + 1;
        
            index++;
        }

        if (ind >= str.Count())
            return;

        label = labels[ind];

        resolve.SetCategoryAndLabel("IDN", label);

        waterMap.ClearAllTiles();

        isWater = false;

    }
}
