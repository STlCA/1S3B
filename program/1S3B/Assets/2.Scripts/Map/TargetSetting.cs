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
    [Header("Object")]
    public GameObject playerObj;
    public GameObject targetSprite;

    private Grid baseGrid;

    private Vector3Int playerCellPosition;
    [HideInInspector] public Vector3Int selectCellPosition;

    private void Start()
    {        
        TempGameManager.instance.targetSetting = this;
        baseGrid = TempGameManager.instance.tileManager.baseGrid;
    }

    private void Update()
    {
        playerCellPosition = baseGrid.WorldToCell(playerObj.transform.position);
    }

    public void SetCellPosition(Vector3 value)
    {
        selectCellPosition = baseGrid.WorldToCell(value);

        TargetUI();
    }

    public bool TargetUI()
    {
        if (TempGameManager.instance.tileManager.isInteractable(selectCellPosition) == false)//밭을 갈수있는 맵이 아니면
        {
            targetSprite.SetActive(false);
            return false;
        }

        if (PlayerBoundCheck() == true)
        {
            targetSprite.SetActive(true);
            TargetPosition();
            return true;
        }
        else
        {
            targetSprite.SetActive(false);
            return false;
        }
    }

    private bool PlayerBoundCheck()
    {
        Vector3 bound = playerCellPosition - selectCellPosition;

        if (-1f <= bound.x && bound.x <= 1f && -1f <= bound.y && bound.y <= 1f)
            return true;
        else
            return false;
    }

    private void TargetPosition()
    {
        gameObject.transform.position = baseGrid.GetCellCenterWorld(selectCellPosition);
    }
}
