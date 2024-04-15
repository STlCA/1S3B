using Constants;
using System;
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
using static UnityEditor.PlayerSettings;

public class PlayerInputController : CharacterEventController
{
    private GameManager gameManager;
    private TileManager tileManager;
    private TargetSetting targetSetting;
    private UIManager uiManager;
    private Player player;
    private PlayerTalkController playerTalkController;
    private AnimationController animController;
    private NatureObjectController natureObjectController;

    private Camera mainCamera;
    private bool isMove;
    private bool isUseEnergy;
    private bool isUseAnim;
    private bool isUse;

    private Vector2 playerPos = new();
    private Vector2 saveDirection = Vector2.zero;


    private void Start()
    {
        mainCamera = Camera.main;
        gameManager = GameManager.Instance;
        tileManager = gameManager.TileManager;
        targetSetting = gameManager.TargetSetting;
        uiManager = gameManager.UIManager;
        player = gameManager.Player;
        natureObjectController = gameManager.NatureObjectController;


        playerTalkController = GetComponent<PlayerTalkController>();
        animController = GetComponent<AnimationController>();
        animController.useAnimEnd += AnimState;
    }

    private void Update()
    {
        if (isUse == true)
        {
            if (isUseAnim == false)
            {
                Use();
            }
        }
    }

    public void AnimState(bool value)
    {
        isUseAnim = value;

        if (value == false)
        {
            if (saveDirection != Vector2.zero)
                CallMoveEvent(saveDirection, true);

            TargetCheck(saveDirection);
            saveDirection = Vector2.zero;
        }
    }

    public bool InputException()
    {
        if (player.playerState == PlayerState.MAPCHANGE)
            return false;
        if (player.playerState == PlayerState.SLEEP)
            return false;

        return true;
    }

    public bool UseException()
    {
        if (InputException() == false)
            return false;

        if (isMove == true)
            return false;

        if (isUseAnim == true)
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

        if (player.playerState == PlayerState.SLEEP && moveInput == Vector2.zero)
            return false;

        if (isUseAnim == true)
        {
            saveDirection = moveInput;
            return false;
        }

        return true;
    }

    private void TargetCheck(Vector2 moveInput)
    {
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
            targetSetting.targetSR.color = new Color(1, 1, 1, 0);
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);

        if (MoveException(moveInput) == false)
            return;

        CallMoveEvent(moveInput);
        TargetCheck(moveInput);
    }

    public void OnMouse(InputValue value)
    {
        if (mainCamera == null && targetSetting == null)
            return;
        if (isMove == true)
            return;

        Vector2 position = value.Get<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(position);

        targetSetting.SetCellPosition(worldPos);
    }
    public void OnHand(InputValue value)//0
    {
        player.currentSelect = PlayerEquipmentType.Hand;
        uiManager.EquipIconChange(PlayerEquipmentType.Hand);
    }
    public void OnHoe(InputValue value)//1
    {
        player.currentSelect = PlayerEquipmentType.Hoe;
        uiManager.EquipIconChange(PlayerEquipmentType.Hoe);
    }
    public void OnWater(InputValue value)//2
    {
        player.currentSelect = PlayerEquipmentType.Water;
        uiManager.EquipIconChange(PlayerEquipmentType.Water);
    }
    public void OnAxe(InputValue value)//3
    {
        player.currentSelect = PlayerEquipmentType.Axe;
        uiManager.EquipIconChange(PlayerEquipmentType.Axe);
    }
    public void OnPickAxe(InputValue value)//4
    {
        player.currentSelect = PlayerEquipmentType.PickAxe;
        uiManager.EquipIconChange(PlayerEquipmentType.PickAxe);
    }
    public void OnSword(InputValue value)//5
    {
        player.currentSelect = PlayerEquipmentType.Sword;
        uiManager.EquipIconChange(PlayerEquipmentType.Sword);
    }
    public void OnSeed(InputValue value)//6
    {
        player.currentSelect = PlayerEquipmentType.Seed;
        uiManager.EquipIconChange(PlayerEquipmentType.Seed);
    }
    public void OnCarry(InputValue value)//7
    {
        player.currentSelect = PlayerEquipmentType.Carry;
        uiManager.EquipIconChange(PlayerEquipmentType.Carry);
    }

    public void OnUse(InputValue value)
    {
        //여기서 들고있는 장비를 부르고 장비에서 갈수있는땅인지 체크하고 장비에서 tillat가고
        //지금은 임시
        //임시 - 여기선 갈땅인지 물줄땅인지만체크
        //메서드로 묶어서 들고있는거별로 다른거 호출하고 거기서 할수있는지 체크?
        //레이를 써서 앞에있을때 그 앞에가 뭐가있을지에 따라 //레이는 마지막인덱스때 콜리더생성        

        Use();

        if (value.isPressed == true)//press진입시간을 줄여서 탭으로만들면?
            isUse = true;
        else if (value.isPressed == false)
            isUse = false;
    }

    private void Use()
    {
        if (UseException() == false)
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        playerPos = transform.position;
        Vector2 pos = new Vector2();

        pos.x = (mousePos.x - playerPos.x);
        pos.y = (mousePos.y - playerPos.y);

        pos = pos.normalized;

        isUseEnergy = false;

        switch (player.currentSelect)
        {
            case PlayerEquipmentType.Hand:
            case PlayerEquipmentType.PickUp://맨손상태 만들기//오른쪽클릭
                UsePickUp(PlayerEquipmentType.PickUp, pos);
                break;
            case PlayerEquipmentType.Hoe:
                UseHoe(PlayerEquipmentType.Hoe, pos);
                break;
            case PlayerEquipmentType.Seed:
                UseSeed(PlayerEquipmentType.Seed, pos);
                break;
            case PlayerEquipmentType.Water:
                UseWater(PlayerEquipmentType.Water, pos);
                break;
            case PlayerEquipmentType.Axe:
                UseAxe(PlayerEquipmentType.Axe, pos);
                break;
            case PlayerEquipmentType.PickAxe:
                UsePickAxe(PlayerEquipmentType.PickAxe, pos);
                break;
            case PlayerEquipmentType.Sword:
                break;
            case PlayerEquipmentType.Carry://언젠간 버리는모션도
                break;
            default:
                break;
        }

        /*if (natureObjectController.IsFelling(targetSetting.selectCellPosition) == true)
        {
            if (isMove == true)
                isUseEnergy = false;
            else
            {
                isUseEnergy = true;
                CallClickEvent(PlayerEquipmentType.Axe, pos);
            }

            natureObjectController.Felling(targetSetting.selectCellPosition);
        }
        else if (natureObjectController.IsMining(targetSetting.selectCellPosition) == true)
        {
            if (isMove == true)
                isUseEnergy = false;
            else
            {
                isUseEnergy = true;
                CallClickEvent(PlayerEquipmentType.PickAxe, pos);
            }

            natureObjectController.Mining(targetSetting.selectCellPosition);
        }
        else if (natureObjectController.IsPickUpNature(targetSetting.selectCellPosition) == true)
        {
            if (isMove == true)
                isUseEnergy = false;
            else
            {
                isUseEnergy = true;
                CallClickEvent(PlayerEquipmentType.PickUp, pos);
            }

            natureObjectController.PickUpNature(targetSetting.selectCellPosition, pos);
        }
        else if (tileManager.IsTilled(targetSetting.selectCellPosition) == false)
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

            tileManager.Harvest(targetSetting.selectCellPosition, pos);
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

            tileManager.WaterAt(targetSetting.selectCellPosition);
        }*/


        if (isUseEnergy == true)
            player.UseEnergy();//씨앗심을때만 빼고 + 장비를 들고있을때만. // 위로올리면 탈진할때 타일에 작용한거 적용이안됨
    }

    private void UsePickAxe(PlayerEquipmentType pickAxe, Vector2 pos)
    {
        isUseEnergy = true;

        if (natureObjectController.IsMining(targetSetting.selectCellPosition) == true)
        {
            CallClickEvent(pickAxe, pos);

            natureObjectController.Mining(targetSetting.selectCellPosition);
        }
    }

    private void UseAxe(PlayerEquipmentType axe, Vector2 pos)
    {
        isUseEnergy = true;

        if (natureObjectController.IsFelling(targetSetting.selectCellPosition) == true)
        {
            CallClickEvent(axe, pos);

            natureObjectController.Felling(targetSetting.selectCellPosition);
        }
    }

    private void UsePickUp(PlayerEquipmentType pickUp, Vector2 pos)
    {
        if (tileManager.IsHarvest(targetSetting.selectCellPosition) == true)//그자리에 작물이 없으면 오류뜰수도
        {
            CallClickEvent(pickUp, pos);

            tileManager.Harvest(targetSetting.selectCellPosition, pos);
        }
        else if (natureObjectController.IsPickUpNature(targetSetting.selectCellPosition) == true)
        {
            CallClickEvent(pickUp, pos);

            natureObjectController.PickUpNature(targetSetting.selectCellPosition, pos);
        }
    }

    private void UseWater(PlayerEquipmentType water, Vector2 pos)
    {
        isUseEnergy = true;

        if (tileManager.IsTilled(targetSetting.selectCellPosition) == true)
        {
            CallClickEvent(water, pos);

            tileManager.WaterAt(targetSetting.selectCellPosition);
        }
    }

    private void UseSeed(PlayerEquipmentType seed, Vector2 pos)
    {
        if (tileManager.IsPlantable(targetSetting.selectCellPosition) == true)
        {
            tileManager.PlantAt(targetSetting.selectCellPosition);
        }
    }

    private void UseHoe(PlayerEquipmentType type, Vector2 pos)
    {
        isUseEnergy = true;

        if (tileManager.IsTilled(targetSetting.selectCellPosition) == false)
        {
            CallClickEvent(type, pos);

            tileManager.TillAt(targetSetting.selectCellPosition);
        }
    }

    public void OnCommunication()
    {
        playerTalkController.NearTalk();
    }



}
