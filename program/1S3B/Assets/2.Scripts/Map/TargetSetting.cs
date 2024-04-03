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
using System;

public class TargetSetting : MonoBehaviour
{
    private GameManager gameManager;
    private TileManager tileManager;

    [Header("Object")]
    public GameObject playerObj;
    public GameObject targetSprite;
    [HideInInspector]public SpriteRenderer targetSR;

    [HideInInspector] public Vector3Int playerCellPosition;
    [HideInInspector] public Vector3Int selectCellPosition;


    private void Start()
    {
        gameManager = GameManager.Instance;
        tileManager = gameManager.TileManager;

        targetSR = targetSprite.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        PlayerSetCellPosition();
    }

    private void PlayerSetCellPosition()
    {
        playerCellPosition = tileManager.baseGrid.WorldToCell(playerObj.transform.position);
    }

    public void SetCellPosition(Vector3 value)
    {
        if (tileManager == null)
            tileManager = GameManager.Instance.TileManager;

        selectCellPosition = tileManager.baseGrid.WorldToCell(value);

        TargetUI();
    }

    public bool TargetUI()
    {
        if (tileManager.IisInteractable(selectCellPosition) == false)//밭을 갈수있는 맵이 아니면
        {
            targetSR.color = new Color(1, 1, 1, 0);
            return false;
        }

        if (PlayerBoundCheck() == true)
        {
            targetSR.color = new Color(1, 1, 1, 1);
            TargetPosition();
            return true;
        }
        else
        {
            targetSR.color = new Color(1, 1, 1, 0);
            return false;
        }
    }

    public bool PlayerBoundCheck()
    {
        Vector3 bound = playerCellPosition - selectCellPosition;

        if (-1f <= bound.x && bound.x <= 1f && -1f <= bound.y && bound.y <= 1f)
            return true;
        else
            return false;
    }

    private void TargetPosition()
    {
        gameObject.transform.position = tileManager.baseGrid.GetCellCenterWorld(selectCellPosition);
    }
}
