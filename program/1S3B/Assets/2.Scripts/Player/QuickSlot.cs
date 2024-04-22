using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlot : MonoBehaviour
{
    // Manager, Data
    GameManager gameManager;
    UIManager uiManager;

    // Script
    Player player;
    public QuickSlotUI quickSlotUI;

    [SerializeField] public Item[] items;
    [SerializeField] public SlotUI[] slots;

    public void Init(GameManager gameManager)
    {
        player = GetComponent<Player>();
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;
        quickSlotUI = uiManager.quickSlotUI;

        // 퀵 슬롯 길이 초기화
        slots = quickSlotUI.GetComponentsInChildren<SlotUI>();
        items = new Item[slots.Length];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new Item();
            slots[i].index = i;
            slots[i].Clear();
            items[i] = slots[i].item;
        }
    }

    // 퀵 슬롯에 아이템 추가
    public bool AddItem(Item item)
    {
        //bool isOwn = IsOwn(item);

        SlotUI emptySlot = GetEmptySlot();

        //if (!isOwn)
        // 넣으려고 하는 아이템이 이미 퀵 슬롯에 존재하는 아이템 아니면
        //if (item.QSymbolActive == false)
        //{
        if (emptySlot != null)
        {
            int idx = emptySlot.index;
            emptySlot.item = item;
            items[idx] = item;
            quickSlotUI.UpdateUI();
            return true;
        }
        //    else
        //    {
        //        return false;
        //    }
        //}
        return false;
    }

    //// 퀵 슬롯에 넣으려고 하는 아이템이 이미 존재하는 아이템인지 확인
    //private bool IsOwn(Item item)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].item == item)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}


    // 비어있는 슬롯 확인
    public SlotUI GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i];
            }
        }

        return null;
    }

    // 퀵 슬롯에 아이템 제거
    public void DeleteItem(Item item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item)
            {
                slots[i].Clear();
                items[i] = null;
                return;
            }
        }

        quickSlotUI.UpdateUI();
    }

    //// UI 업데이트
    //private void UpdateUI()
    //{
    //    for (int i = 0; i < items.Length; i++)
    //    {
    //        if (items[i] != null)
    //        {
    //            slots[i].Set();
    //        }
    //        else
    //        {
    //            slots[i].Clear();
    //        }
    //    }
    //}

    // 아이템 사용
    private void UseItem()
    {

    }
}
