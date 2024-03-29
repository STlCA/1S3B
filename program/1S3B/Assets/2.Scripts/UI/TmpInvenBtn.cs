using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpInvenBtn : MonoBehaviour
{
    [SerializeField] private GameObject inventoryUI;
    GameManager gameManager;
    UIManager uiManager;
    DataManager dataManager;
    Inventory inventory;

    private void Start()
    {
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;
        dataManager = gameManager.DataManager;
        inventory = gameManager.Player.Inventory;
    }

    public void OnClickInvenBtn()
    {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }

    public void OnClickAddItem()
    {
        // dataManager.itemDatabase.ItemData[Random.Range(0, dataManager.itemDatabase.ItemData.Count)]
        // 테스트 아이템 만들기
        Item item = dataManager.itemDatabase.Gacha();
        if (!inventory.AddItem(item))
        {
            // 실패
        }      

    }
}
