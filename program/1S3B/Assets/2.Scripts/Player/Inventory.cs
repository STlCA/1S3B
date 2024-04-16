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

    [SerializeField] List<Item> items = new();
    public List<Item> Items { get { return items; } }

    [Header("Selected Item")]
    private InventorySlotUI _selectedItem;
    private int _selectedItemIndex;

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
            Item slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                // UpdateUI();
                // inventoryUI.Refresh();
                return true;
            }
            AddNewItem(item);
            return true;
        }
        else
        {
            AddNewItem(item);


            return true;
        }

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
        if(items.Count <= idx)
        {
            return null;
        }

        return items[idx];
    }

    // 아이템 선택
    public void SelectItem(InventorySlotUI _item)
    {
        _selectedItem = _item;
        _selectedItemIndex = _item.index;

        string infoString = "";

        for (int i = 0; i < _selectedItem._item.ItemInfo.Description.Count; i++)
        {
            infoString += _selectedItem._item.ItemInfo.Description[i];
        }

        _selectedItem.UpdateItemInfo(_selectedItem._item.ItemInfo.Name, infoString);
    }
}
