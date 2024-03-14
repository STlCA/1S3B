using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerInputController : CharacterEventController
{
    private Camera mainCamera;
    private TargetSetting targetSetting;
    private TileManager tileManager;

    private void Start()
    {
        mainCamera = Camera.main;
        targetSetting = TempGameManager.instance.targetSetting;
        tileManager = TempGameManager.instance.tileManager;
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);        

        if (Keyboard.current.aKey.isPressed == true && Keyboard.current.dKey.isPressed == true)
            return;
        if (Keyboard.current.wKey.isPressed == true && Keyboard.current.sKey.isPressed == true)
            return;

        CallMoveEvent(moveInput);

        //움직이면 타겟안보임
        if (moveInput == Vector2.zero)
            targetSetting.gameObject.SetActive(true);
        else
            targetSetting.gameObject.SetActive(false);
    }

    public void OnMouse(InputValue value)
    {
        Vector2 position = value.Get<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(position);

        targetSetting.SetCellPosition(worldPos);
    }

    public void OnInteraction(InputValue value)
    {
        //여기서 들고있는 장비를 부르고 장비에서 갈수있는땅인지 체크하고 장비에서 tillat가고
        //지금은 임시
        //임시 - 여기선 갈땅인지 물줄땅인지만체크
        if (tileManager.IsTilled(targetSetting.selectCellPosition) == false)
        {
            //애니메이션
            tileManager.TillAt(targetSetting.selectCellPosition);
        }
        else if (tileManager.IsTilled(targetSetting.selectCellPosition) == true)
        {
            //애니메이션
            tileManager.WaterAt(targetSetting.selectCellPosition);
        }
    }
}
