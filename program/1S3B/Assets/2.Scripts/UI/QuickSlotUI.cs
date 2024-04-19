using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    //// Manager, Data
    //GameManager gameManager;
    //UIManager uiManager;
    //DataManager dataManager;
    //ItemDatabase itemDatabase;

    //// Script
    //Player player;
    //Inventory inventory;
    //SlotUI slotUI;

    public void Init()
    {
        //this.player = player;
        //inventory = player.Inventory;

        //this.gameManager = gameManager;
        //this.uiManager = uiManager;
        //dataManager = gameManager.DataManager;

        //itemDatabase = dataManager.itemDatabase;

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
