using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class PlayerInputController : CharacterEventController
{
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
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
            GameManager.Instance.targetSetting.gameObject.SetActive(true);
        else
            GameManager.Instance.targetSetting.gameObject.SetActive(false);
    }

    public void OnMouse(InputValue value)
    {
        if (mainCamera == null && GameManager.Instance.targetSetting == null)
            return;

        Vector2 position = value.Get<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(position);
        
        GameManager.Instance.targetSetting.SetCellPosition(worldPos);

    }


    public void OnUse(InputValue value)
    {
        //여기서 들고있는 장비를 부르고 장비에서 갈수있는땅인지 체크하고 장비에서 tillat가고
        //지금은 임시
        //임시 - 여기선 갈땅인지 물줄땅인지만체크
        //메서드로 묶어서 들고있는거별로 다른거 호출하고 거기서 할수있는지 체크?

        if (GameManager.Instance.tileManager.IsTilled(GameManager.Instance.targetSetting.selectCellPosition) == false)
        {


            GameManager.Instance.tileManager.TillAt(GameManager.Instance.targetSetting.selectCellPosition);
        }
        else if (GameManager.Instance.tileManager.IsPlantable(GameManager.Instance.targetSetting.selectCellPosition) == true)
        {


            GameManager.Instance.tileManager.PlantAt(GameManager.Instance.targetSetting.selectCellPosition);
        }
        else if (GameManager.Instance.tileManager.IsHarvest(GameManager.Instance.targetSetting.selectCellPosition) == true)
        //레이를 써서 앞에있을때 그 앞에가 뭐가있을지에 따라 //레이는 마지막인덱스때 콜리더생성
        {


            GameManager.Instance.tileManager.Harvest(GameManager.Instance.targetSetting.selectCellPosition);
        }
        else if (GameManager.Instance.tileManager.IsTilled(GameManager.Instance.targetSetting.selectCellPosition) == true)
        {


            GameManager.Instance.tileManager.WaterAt(GameManager.Instance.targetSetting.selectCellPosition);
        }

        if (true)
            PlayerStatus.player.UseEnergy();//씨앗심을때만 빼고 + 장비를 들고있을때만. // 위로올리면 탈진할때 타일에 작용한거 적용이안됨
    }

    public void OnCommunication(InputValue value)
    {

    }



}
