using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpInvenBtn : MonoBehaviour
{
    [SerializeField] private InventoryUI inventoryUI;
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
        inventoryUI.InventoryEnable();
    }

    public void OnClickAddItem()
    {
        // dataManager.itemDatabase.ItemData[Random.Range(0, dataManager.itemDatabase.ItemData.Count)]
        // �׽�Ʈ ������ �����
        Item item = dataManager.itemDatabase.Gacha();
        // Debug.Log(item.ItemInfo.Name);

        if (!inventory.AddItem(item))
        {
            Debug.Log("아이템 획득 실패");
            return;
        }
        // inventory.AddItem(item);
        // Debug.Log(item.ItemInfo.Name);

    }
}
