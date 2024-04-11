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
    InventoryUI inventoryUI;
    Player player;

    [SerializeField] List<Item> items = new();
    public List<Item> Items { get { return items; } }

    //[Header("Selected Item")]
    //private InventorySlotUI _selectedItem;
    //private int _selectedItemIndex;

    private void Start()
    {
        player = GetComponent<Player>();
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;
        inventoryUI = uiManager.inventoryUI;
    }

    // 아이템 추가
    public bool AddItem(Item item)
    {
        // 스택
        // 더 이상 가질수 있는지 없는지
        // return false
        Item emptySlot = GetEmptySlot();

        // 스택 가능한 아이템
        if (item.ItemInfo.canStack)
        {
            Item slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                //UpdateUI();
                return true;
            }
        }
        // 스택이 안되는 아이템
        else
        {
            emptySlot = item;
            emptySlot.quantity = 1;
            inventoryUI.Refresh();
            return true;
        }
        //inventoryUI.Refresh();
        return false;
    }

    // 획득한 아이템이 기존에 획득했것인지 확인
    Item GetItemStack(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == item && items[i].quantity < item.ItemInfo.Stack)
            {
                return items[i];
            }
        }

        return null;
    }

    // 비어있는 슬롯 확인
    public Item GetEmptySlot()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                return items[i];
        }

        return null;
    }

    // 아이템 제거
    private void RemoveSelectedItem(Item item)
    {
        items.Remove(item);

        //_selectedItem.quantity--;

        //    if (_selectedItem.quantity <= 0)
        //    {
        //        _selectedItem.iteminstance.item = null;
        //    }

        //    UpdateUI();
    }

    // 아이템 선택
    public Item GetItem(int idx)
    {
        // 예외처리
        return items[idx];
    }

    //// 아이템 선택
    //public void SelectItem(int index)
    //{
    //    _selectedItem = slots[index];
    //    _selectedItemIndex = index;
    //    string infoString = "";

    //    for (int i = 0; i < _selectedItem.iteminstance.item.Description.Count; i++)
    //    {
    //        infoString += _selectedItem.iteminstance.item.Description[i];
    //    }

    //    uiSlots[index].UpdateItemInfo(_selectedItem.iteminstance.item.Name, infoString);
    //}
}
