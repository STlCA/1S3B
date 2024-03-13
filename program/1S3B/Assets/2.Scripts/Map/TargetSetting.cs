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
}
