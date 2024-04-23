using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인벤토리 스크립트 [모델]
// 데이터를 넣고/빼는 것 관리
// 아이템이 스택 가능한지 확인

public class Inventory : MonoBehaviour
{
    // Manager
    GameManager gameManager;
    UIManager uiManager;

    // Script
    public InventoryUI inventoryUI;
    Player player;
    QuickSlot quickSlot;

    [SerializeField] List<Item> items = new();
    public List<Item> Items { get { return items; } }

    [SerializeField] private QuickSlotUI quickSlotUI;

    [Header("Selected Item")] // 마우스가 가리키는 아이템
    private InventorySlotUI _selectedItem;
    private int _selectedItemIndex;

    InventorySlotUI selectedSlotUI; // 클릭한 아이템

    ItemDatabase database;

    public void Init(GameManager gameManager)
    {
        player = GetComponent<Player>();
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;
        inventoryUI = uiManager.inventoryUI;
        quickSlot = player.QuickSlot;
        database = gameManager.DataManager.itemDatabase;
    }
    private void Start()
    {
        
        UseItemInit();
    }

    private void UseItemInit()
    {
        ItemInfo itemInfo = database.GetItemByKey(1001);
        Item item = new Item();
        item.ItemInfo = itemInfo;
        AddItem(item);

        itemInfo = database.GetItemByKey(1002);
        item = new Item();
        item.ItemInfo = itemInfo;
        AddItem(item);

        itemInfo = database.GetItemByKey(1003);
        item = new Item();
        item.ItemInfo = itemInfo;
        AddItem(item);

        itemInfo = database.GetItemByKey(1004);
        item = new Item();
        item.ItemInfo = itemInfo;
        AddItem(item);
    }

    // 아이템 추가
    public bool AddItem(Item item)
    {
        // 스택
        // 더 이상 가질수 있는지 없는지
        // return false

        // 스택 가능한 아이템
        if (item.ItemInfo.canStack)
        {
            Item slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                // UpdateUI();
                // inventoryUI.Refresh();
                quickSlotUI.UpdateUI(); // **************** TODO : 임시로 만든 아이템 넣는 버튼 없앨 때 아랫 줄의 리턴과 함께 삭제
                return true;
            }
            AddNewItem(item);
            quickSlotUI.UpdateUI(); // **************** TODO : 임시로 만든 아이템 넣는 버튼 없앨 때 아랫 줄의 리턴과 함께 삭제
            return true;
        }
        else
        {
            AddNewItem(item);

            quickSlotUI.UpdateUI(); // **************** TODO : 임시로 만든 아이템 넣는 버튼 없앨 때 아랫 줄의 리턴과 함께 삭제
            return true;
        }

        quickSlotUI.UpdateUI();
        //Item emptySlot = GetEmptySlot();
        // 스택이 안되는 아이템이거나 새로운 아이템
        //if(emptySlot != null)
        //{
        //    emptySlot = item;
        //    emptySlot.quantity = 1;
        //    inventoryUI.Refresh();

        //    Debug.Log(item.ItemInfo.Name + "확인");


        //    return true;
        //}

        //inventoryUI.Refresh();
        //return false;
    }

    public void AddNewItem(Item item)
    {
        items.Add(item);
        item.quantity = 1;
        inventoryUI.Refresh();

        Debug.Log(item.ItemInfo.Name + "확인");
    }

    // 획득한 아이템이 기존에 획득했것인지 확인
    Item GetItemStack(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ItemInfo.ID == item.ItemInfo.ID && items[i].quantity < item.ItemInfo.Stack)
            {
                return items[i];
            }
        }

        return null;
    }

    // 비어있는 슬롯 확인
    public Item GetEmptySlot()
    {
        // *********** TODO : 슬롯 길이 얼나마 할지 정해서 슬롯 길이로 바꾸고 활정화 *************************
        //for (int i = 0; i < items.Count; i++)
        //{
        //    if (items[i] == null)
        //        return items[i];
        //}

        //return null;
        return items[items.Count + 1];
    }

    // 아이템 선택
    public Item GetItem(int idx)
    {
        // 예외처리
        if(items.Count <= idx)
        {
            return null;
        }

        return items[idx];
    }

    // 아이템 선택
    public void SelectItem(InventorySlotUI item)
    {
        _selectedItem = item;
        _selectedItemIndex = item.index;

        string infoString = "";

        for (int i = 0; i < _selectedItem._item.ItemInfo.Description.Count; i++)
        {
            infoString += _selectedItem._item.ItemInfo.Description[i];
        }

        _selectedItem.UpdateItemInfo(_selectedItem._item.ItemInfo.Name, infoString);
    }

    //// 아이템 사용
    //public void UseItem(InventorySlotUI slot, Item item)
    //{
    //    //_selectedItem = item;
    //    //_selectedItemIndex = item.index;

    //    // 아이템이 장착 아이템이라면
    //    if (item.ItemInfo.Type == "Equip")
    //    {
    //        EquipItem();
    //        return;
    //    }

    //    item.quantity--;

    //    if (item.quantity <= 0)
    //    {
    //        RemoveSelectedItem(slot, item);
    //    }
    //}

    #region 퀵 슬롯 관련
    // 아이템을 퀵 슬롯에 할당
    //public void OnClickButtonQuickApply(InventorySlotUI slot, Item item)
    //{
    //    if(item.ItemInfo.Type == "Equip" || item.ItemInfo.Type == "Crop")
    //    {
    //        quickSlot.AddItem(item);
    //    }
    //}

    public void SelectSlot(InventorySlotUI slot)
    {
        if(selectedSlotUI != null)
        {
            // 이전 슬롯 아웃라인 비활성화
            selectedSlotUI.OutlineDisable();
        }

        selectedSlotUI = slot;
    }

    // 퀵 슬롯에 넣는 버튼
    public void OnClickApplyBtn()
    {
        if(selectedSlotUI == null)
        {
            return;
        }

        Item item = selectedSlotUI._item;

        // 퀵슬롯에 넣을 수 있는 아이템인지 검사
        if (item.ItemInfo.Type == "Equip" || item.ItemInfo.Type == "Seed")
        {
            // 넣으려고 하는 아이템이 퀵 슬롯에 없는 아이템이면
            // 해당 아이템 퀵 슬롯에 추가
            if (item.QSymbolActive != true)
            {
                if (!quickSlot.AddItem(item))
                {
                    return;
                }
                item.QSymbolActive = true;
            }
            // 해당 아이템 퀵 슬롯에서 제거
            else
            {
                quickSlot.DeleteItem(item);
                item.QSymbolActive = false;
            }
        }

        inventoryUI.Refresh();
    }

    // 아이템 장착
    public void EquipItem()
    {

    }

    //// 아이템 퀵슬롯에 장착
    //public void InputQuickSlot(InventorySlotUI slot, Item item)
    //{
    //    //Item tmpItem = item;
    //    RemoveSelectedItem(slot, item);
    //    //return tmpItem;
    //}
    #endregion // 퀵 슬롯 관련

    #region 아이템 판매 관련
    // 선택한 아이템 판매
    public void OnClickSellBtn()
    {
        if (selectedSlotUI == null)
        {
            return;
        }

        Item item = selectedSlotUI._item;

        // 장착 아이템 제외 모든 아이템 판매 가능
        if(item.ItemInfo.Type != "Equip")
        {
            // 수량 입력 창 만들면 주석 해제하고 밑에 있는 코드 삭제
            //player.Deposit(item.ItemInfo.SellGold, item.quantity);
            //RemoveSelectedItem(selectedSlotUI, item);
            player.Deposit(item.ItemInfo.SellGold);
            item.quantity--;

            if(item.quantity <= 0)
            {
                RemoveSelectedItem(selectedSlotUI, item);
                return;
            }
            inventoryUI.Refresh();
        }

    }

    // 아이템 제거
    private void RemoveSelectedItem(InventorySlotUI slot, Item item)
    {
        items.Remove(item);
        slot.Clear();

        inventoryUI.Refresh();
        //_selectedItem.quantity--;

        //    if (_selectedItem.quantity <= 0)
        //    {
        //        _selectedItem.iteminstance.item = null;
        //    }

        //    UpdateUI();
    }
    #endregion // 아이템 판매 관련
}
