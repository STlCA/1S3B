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
        // ����
        // ���̻� ������ �ִ��� ������
        // return false

        items.Add(item);
        inventoryUI.Refresh();
        return true;
    }

}
