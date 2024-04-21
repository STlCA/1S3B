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
using static UnityEngine.Rendering.ReloadAttribute;

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

        if (value == false)//장비사용중에 이동키를 누르고있으면 애니메이션이 끝나자마자 이동하게
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

        //if (player.currentSelectType == PlayerEquipmentType.Carry)
        if (player.selectItem != null && (player.selectItem.ItemInfo.Type == "Item" || player.selectItem.ItemInfo.Type == "Crop"))
            CallMoveEvent(moveInput, false, true);
        else
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
    //public void OnHand(InputValue value)//0
    //{
    //    uiManager.EquipIconChange(PlayerEquipmentType.Hand);
    //
    //    animController.CarryAnimation(false);
    //
    //    player.selectItem = null;
    //}

    private void QuickSlotItemCheck(int index)
    {
        if(player.QuickSlot.slots[index].item == null)
        {
            player.selectItem = null;
            return;
        }

        Item item = player.QuickSlot.slots[index].item;
        player.selectItem = item;

        animController.CarryAnimation(item.ItemInfo.Type != "Equip");
    }

    public void OnOne(InputValue value)//1
    {
        QuickSlotItemCheck(0);        

        //uiManager.EquipIconChange(PlayerEquipmentType.Hoe);
        //ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(1001);
        //player.selectItem = iteminfo;
        //슬롯선택 선택을했을때 그 슬롯의 아이템인포를 가져와야
    }
    public void OnTwo(InputValue value)//2
    {
        QuickSlotItemCheck(1);
/*        uiManager.EquipIconChange(PlayerEquipmentType.Water);

        animController.CarryAnimation(false);

        ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(1002);
        player.selectItem = iteminfo;*/
    }
    public void OnThree(InputValue value)//3
    {
        QuickSlotItemCheck(2);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.Axe);

                animController.CarryAnimation(false);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(1004);
                player.selectItem = iteminfo;*/
    }
    public void OnFour(InputValue value)//4
    {
        QuickSlotItemCheck(3);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.PickAxe);

                animController.CarryAnimation(false);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(1003);
                player.selectItem = iteminfo;*/
    }
    public void OnFive(InputValue value)//5
    {
        QuickSlotItemCheck(4);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.Sword);

                animController.CarryAnimation(false);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(1005);
                player.selectItem = iteminfo;*/
    }
    public void OnSix(InputValue value)//6
    {
        QuickSlotItemCheck(5);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.Seed);

                animController.CarryAnimation(false);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(2001);
                player.selectItem = iteminfo;*/
    }
    public void OnSeven(InputValue value)//7
    {
        QuickSlotItemCheck(6);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.Carry);

                animController.CarryAnimation(true);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(5002);
                player.selectItem = iteminfo;*/
    }
    public void OnEight(InputValue value)//8
    {
        QuickSlotItemCheck(7);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.Carry);

                animController.CarryAnimation(true);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(5002);
                player.selectItem = iteminfo;*/
    }
    public void OnNine(InputValue value)//9
    {
        QuickSlotItemCheck(8);
        /*        uiManager.EquipIconChange(PlayerEquipmentType.Carry);

                animController.CarryAnimation(true);

                ItemInfo iteminfo = gameManager.DataManager.itemDatabase.GetItemByKey(5002);
                player.selectItem = iteminfo;*/
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

        if (player.selectItem != null)
        {
            switch (player.selectItem.ItemInfo.Type)
            {
                case "Crop":
                    UseSeed(PlayerEquipmentType.Seed, pos);
                    break;

                case "Equip":
                    switch (player.selectItem.ItemInfo.EquipType)
                    {
                        case "Hoe":
                            UseHoe(PlayerEquipmentType.Hoe, pos);
                            break;
                        case "Water":
                            UseWater(PlayerEquipmentType.Water, pos);
                            break;
                        case "Axe":
                            UseAxe(PlayerEquipmentType.Axe, pos);
                            break;
                        case "PickAxe":
                            UsePickAxe(PlayerEquipmentType.PickAxe, pos);
                            break;
                    }
                    break;

                    /*case PlayerEquipmentType.Water:

                    case PlayerEquipmentType.Axe:

                    case PlayerEquipmentType.PickAxe:

                    case PlayerEquipmentType.Carry://언젠간 버리는모션도
                        break;
                    case PlayerEquipmentType.Sword:
                        break;
                    default:
                        break;*/
            }
        }
        else
            UsePickUp(PlayerEquipmentType.PickUp, pos);

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

    public void OnEquipPickUp(InputValue value)
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

        UsePickUp(PlayerEquipmentType.PickUp, pos);
    }

    private void UsePickAxe(PlayerEquipmentType pickAxe, Vector2 pos)
    {
        Vector3Int target = targetSetting.selectCellPosition;

        isUseEnergy = true;
        CallClickEvent(pickAxe, pos);

        if (natureObjectController.IsMining(target) == true)
            natureObjectController.Mining(target);

        else if (tileManager.IsPlant(target) == true)
            tileManager.DestroyCropData(target);//작물파괴

        else if (tileManager.IsPlantable(target) == true)
            tileManager.DestroyGroundData(target);//땅파괴
    }

    private void UseAxe(PlayerEquipmentType axe, Vector2 pos)
    {
        isUseEnergy = true;
        CallClickEvent(axe, pos);

        if (natureObjectController.IsFelling(targetSetting.selectCellPosition) == true)
            natureObjectController.Felling(targetSetting.selectCellPosition);
        else
            CallClickEvent(axe, pos);
    }

    private void UsePickUp(PlayerEquipmentType pickUp, Vector2 pos)
    {
        if (tileManager.IsHarvest(targetSetting.selectCellPosition) == true)//그자리에 작물이 없으면 오류뜰수도
        {
            CallClickEvent(pickUp, pos);//PickUp은 callClick이 안으로 들어와야 아무것도 없을떄 행동 안함
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
        CallClickEvent(water, pos);

        if (tileManager.IsTilled(targetSetting.selectCellPosition) == true)
            tileManager.WaterAt(targetSetting.selectCellPosition);
    }

    private void UseSeed(PlayerEquipmentType seed, Vector2 pos)
    {
        if (tileManager.IsPlantable(targetSetting.selectCellPosition) == true)
            tileManager.PlantAt(targetSetting.selectCellPosition,player.selectItem.ItemInfo);
    }

    private void UseHoe(PlayerEquipmentType type, Vector2 pos)
    {
        isUseEnergy = true;
        CallClickEvent(type, pos);

        if (tileManager.IsTilled(targetSetting.selectCellPosition) == false)
            tileManager.TillAt(targetSetting.selectCellPosition);
    }

    public void OnCommunication()
    {
        playerTalkController.NearTalk();
    }



}
