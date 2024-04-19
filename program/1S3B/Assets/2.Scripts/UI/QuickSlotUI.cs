using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    // Manager, Data
    GameManager gameManager;
    UIManager uiManager;
    DataManager dataManager;
    ItemDatabase itemDatabase;

    // Script
    Player player;
    Inventory inventory;
    SlotUI slotUI;

    [SerializeField] private SlotUI[] slots;

    public void Init(GameManager gameManager, UIManager uiManager, Player player)
    {
        this.player = player;
        inventory = player.Inventory;

        this.gameManager = gameManager;
        this.uiManager = uiManager;
        dataManager = gameManager.DataManager;

        itemDatabase = dataManager.itemDatabase;

        // 퀵 슬롯 길이 초기화
        slots = new SlotUI[9];

        gameObject.SetActive(true);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
