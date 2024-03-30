using Constants;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class PlayerInputController : CharacterEventController
{
    private GameManager gameManager;
    private TileManager tileManager;
    private TargetSetting targetSetting;
    private Player player;
    private PlayerTalkController playerTalkController;

    private Camera mainCamera;
    private bool isMove;
    private bool isUseEnergy;
    //private bool isUseAnim = false;

    private Vector2 playerPos = new();


    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = GameManager.Instance;
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        player = gameManager.Player;
        
        playerTalkController = GetComponent<PlayerTalkController>();
    }

    public bool InputException()
    {
        if (player.playerState == PlayerState.MAPCHANGE)
            return false;
        if (player.playerState == PlayerState.SLEEP)
            return false;

        return true;
    }

    public bool MoveException(Vector2 moveInput)
    {
        if (Keyboard.current.aKey.isPressed == true && Keyboard.current.dKey.isPressed == true)
            return false;
        if (Keyboard.current.wKey.isPressed == true && Keyboard.current.sKey.isPressed == true)
            return false;

        if (InputException() == false && moveInput != Vector2.zero)
            return false;

        return true;
    }

    //public void UseAnimEnd()
    //{
    //    CallMoveEvent(player.saveDirection);
    //    isUseAnim = false;
    //}

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);

        if (MoveException(moveInput) == false)
            return;

        //if (isUseAnim == true)
        //{            
        //    player.SaveDirectionSet(moveInput);
        //    return;            
        //}

        CallMoveEvent(moveInput);

        //움직이면 타겟안보임
        if (moveInput == Vector2.zero)
        {
            isMove = false;
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            targetSetting.SetCellPosition(mousePos);
        }
        else
        {
            isMove = true;
            targetSetting.targetSprite.SetActive(false);
        }
    }

    public void OnMouse(InputValue value)
    {
        if (mainCamera == null && targetSetting == null)
            return;

        Vector2 position = value.Get<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(position);

        targetSetting.SetCellPosition(worldPos);
    }


    public void OnUse(InputValue value)
    {
        //여기서 들고있는 장비를 부르고 장비에서 갈수있는땅인지 체크하고 장비에서 tillat가고
        //지금은 임시
        //임시 - 여기선 갈땅인지 물줄땅인지만체크
        //메서드로 묶어서 들고있는거별로 다른거 호출하고 거기서 할수있는지 체크?
        //레이를 써서 앞에있을때 그 앞에가 뭐가있을지에 따라 //레이는 마지막인덱스때 콜리더생성

        if (InputException() == false)
            return;

        //isUseAnim = true;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        playerPos = transform.position;
        Vector2 pos = new Vector2();

        pos.x = (mousePos.x - playerPos.x);
        pos.y = (mousePos.y - playerPos.y);

        pos.Normalize();

        isUseEnergy = false;

        if (tileManager.IsTilled(targetSetting.selectCellPosition) == false)
        {
            if (isMove == true)
                isUseEnergy = false;
            else
            {
                isUseEnergy = true;
                CallClickEvent(PlayerEquipmentType.Hoe, pos);
            }

            tileManager.TillAt(targetSetting.selectCellPosition);
        }
        else if (tileManager.IsPlantable(targetSetting.selectCellPosition) == true)
        {


            tileManager.PlantAt(targetSetting.selectCellPosition);
        }
        else if (tileManager.IsHarvest(targetSetting.selectCellPosition) == true)
        {
            if (isMove == true)
                isUseEnergy = false;
            else
            {
                isUseEnergy = true;
                CallClickEvent(PlayerEquipmentType.PickUp, pos);
            }

            tileManager.Harvest(targetSetting.selectCellPosition);
        }
        else if (tileManager.IsTilled(targetSetting.selectCellPosition) == true)
        {
            if (isMove == true)
                isUseEnergy = false;
            else
            {
                isUseEnergy = true;
                CallClickEvent(PlayerEquipmentType.Water, pos);
            }

            GameManager.Instance.TileManager.WaterAt(targetSetting.selectCellPosition);
        }

        if (isUseEnergy == true)
            player.UseEnergy();//씨앗심을때만 빼고 + 장비를 들고있을때만. // 위로올리면 탈진할때 타일에 작용한거 적용이안됨
    }

    public void OnCommunication()
    {
        playerTalkController.NearTalk();
    }



}
