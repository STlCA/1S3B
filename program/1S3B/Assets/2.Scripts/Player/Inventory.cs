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

    [Header("Selected Item")]
    private InventorySlotUI _selectedItem;
    private int _selectedItemIndex;

    public void Init(GameManager gameManager)
    {
        player = GetComponent<Player>();
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;
        inventoryUI = uiManager.inventoryUI;
        quickSlot = player.QuickSlot;
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

    // 아이템 사용
    public void UseItem(InventorySlotUI slot, Item item)
    {
        //_selectedItem = item;
        //_selectedItemIndex = item.index;

        // 아이템이 장착 아이템이라면
        if (item.ItemInfo.Type == "Equip")
        {
            EquipItem();
            return;
        }

        item.quantity--;

        if (item.quantity <= 0)
        {
            RemoveSelectedItem(slot, item);
        }
    }

    // 아이템을 퀵 슬롯에 할당
    //public void OnClickButtonQuickApply(InventorySlotUI slot, Item item)
    //{
    //    if(item.ItemInfo.Type == "Equip" || item.ItemInfo.Type == "Crop")
    //    {
    //        quickSlot.AddItem(item);
    //    }
    //}

    InventorySlotUI selectedSlotUI;
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

        if (item.ItemInfo.Type == "Equip" || item.ItemInfo.Type == "Crop")
        {
            if (!quickSlot.AddItem(item))
            {
                return;
            }
            item.QSymbolActive = true;
        }

        inventoryUI.Refresh();
    }

    // 아이템 장착
    public void EquipItem()
    {

    }

    // 아이템 제거
    private void RemoveSelectedItem(InventorySlotUI slot, Item item)
    {
        items.Remove(item);
        slot.Clear();

        //_selectedItem.quantity--;

        //    if (_selectedItem.quantity <= 0)
        //    {
        //        _selectedItem.iteminstance.item = null;
        //    }

        //    UpdateUI();
    }

    // 아이템 퀵슬롯에 장착
    public void InputQuickSlot(InventorySlotUI slot, Item item)
    {
        //Item tmpItem = item;
        RemoveSelectedItem(slot, item);
        //return tmpItem;
    }
}
