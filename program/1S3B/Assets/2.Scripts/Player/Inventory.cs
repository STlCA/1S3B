using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    Player player;
    [SerializeField] List<Item> items = new();
    public List<Item> Items { get { return items; } }

    GameManager gameManager;
    UIManager uiManager;
    InventoryUI inventoryUI;
    private void Start()
    {
        player = GetComponent<Player>();
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;
        inventoryUI = uiManager.inventoryUI;
    }


    public bool AddItem(Item item)
    {
        // 스택
        // 더이상 가질수 있는지 없는지
        // return false

        items.Add(item);
        inventoryUI.Refresh();
        return true;
    }

}
