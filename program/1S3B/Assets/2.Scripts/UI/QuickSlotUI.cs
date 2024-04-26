using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    // Manager, Data
    GameManager gameManager;
    UIManager uiManager;
    //DataManager dataManager;
    //ItemDatabase itemDatabase;

    // Script
    Player player;
    //Inventory inventory;
    QuickSlot quickSlot;
    SlotUI slotUI;

    public void Init(GameManager gameManager, UIManager uiManager, Player player)
    {
        this.player = player;
        quickSlot = player.QuickSlot;

        this.gameManager = gameManager;
        this.uiManager = uiManager;
        //dataManager = gameManager.DataManager;

        //itemDatabase = dataManager.itemDatabase;

        //slotUI.StateInit();

        QuickSlotEnable();
    }

    // 퀵 슬롯 활성화
    public void QuickSlotEnable()
    {
        gameObject.SetActive(true);
    }

    // 퀵 슬롯 비활성화
    public void QuickSlotDisable()
    {
        gameObject.SetActive(false);
    }

    // UI 업데이트
    public void UpdateUI()
    {
        for (int i = 0; i < quickSlot.items.Length; i++)
        {
            if (quickSlot.items[i] != null)
            {
                quickSlot.slots[i].Set();
            }
            else
            {
                quickSlot.slots[i].Clear();
            }
        }
    }
}
