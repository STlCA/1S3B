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

        // 스택 가능한 아이템
        if (item.ItemInfo.canStack)
        {

        }
        // 스택이 안되는 아이템
        else
        {
            items.Add(item);
        }
        inventoryUI.Refresh();
        return true;
    }

    // 아이템 제거
    private void RemoveSelectedItem()
    {

    }

    public Item GetItem(int idx)
    {
        // 예외처리
        return items[idx];
    }


    //// 인벤토리에 아이템 추가
    //public void AddItem(ItemInfo item)
    //{
    //    // 아이템이 도구가 아닐 때
    //    if (item.canStack)
    //    {
    //        ItemSlot slotToStackTo = GetItemStack(item);
    //        if (slotToStackTo != null)
    //        {
    //            slotToStackTo.quantity++;
    //            UpdateUI();
    //            return;
    //        }
    //    }

    //    ItemSlot emptySlot = GetEmptySlot();

    //    if (emptySlot != null)
    //    {
    //        emptySlot.iteminstance.item = item;
    //        emptySlot.quantity = 1;
    //        UpdateUI();
    //        return;
    //    }
    //}

    //// 획득한 아이템이 기존에 획득했것인지 확인
    //ItemSlot GetItemStack(ItemInfo item)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].iteminstance.item == item && slots[i].quantity < item.Stack)
    //        {
    //            return slots[i];
    //        }
    //    }

    //    return null;
    //}

    //// 비어있는 슬롯 확인
    //public ItemSlot GetEmptySlot()
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].iteminstance.item == null)
    //            return slots[i];
    //    }

    //    return null;
    //}

    //// UI 업데이트
    //void UpdateUI()
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].iteminstance.item != null)
    //        {
    //            uiSlots[i].Set(slots[i]);
    //        }
    //        else
    //        {
    //            uiSlots[i].Clear();
    //        }
    //    }
    //}

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

    //// 아이템 제거
    //private void RemoveSelectedItem()
    //{
    //    _selectedItem.quantity--;

    //    if (_selectedItem.quantity <= 0)
    //    {
    //        _selectedItem.iteminstance.item = null;
    //    }

    //    UpdateUI();
    //}
}
