using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TargetSetting : MonoBehaviour
{
    [Header("PlayerObject")]
    public GameObject playerObj;
    public GameObject targetSprite;

    [Header("TileMap")]
    public Tilemap backgroundMap;
    public Tilemap interactableMap;
    public Tile interactableTile;

    private Vector3Int playerCellPosition;
    private Vector3Int selectCellPosition;

    private void Update()
    {
        if (interactableMap != null)
            playerCellPosition = interactableMap.WorldToCell(playerObj.transform.position);
    }

    public void SetCellPosition(Vector3 value)
    {
        if (interactableMap == null)
            return;

        selectCellPosition = interactableMap.WorldToCell(value);

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

        if (interactableMap.GetTile(selectCellPosition) == interactableTile)
        {
            Debug.Log("맞다야");
            backgroundMap.SetTile(selectCellPosition, interactableTile);
        }
    }

}
