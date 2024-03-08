using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemImg : MonoBehaviour
{
    // ���� �ν�����
    EventTrigger ev;

    // �ܺ� ������Ʈ
    GameManager gameManager;

    // ����
    public int type;
    public int count;
    public int sellPrice;
    public string originalName;
    public string koreanName;
    public Chest myChest;
    public TextMeshProUGUI text_count;

    bool isClickLeft;
    bool isClickRight;

    void Start()
    {
        ev = GetComponent<EventTrigger>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((e) =>
        {
            if (isClickLeft) // ��Ŭ��
                LeftClickItem();
            else if (isClickRight) // ��Ŭ��
                RightClickItem();
        });
        ev.triggers.Add(entry);
    }

    private void Update()
    {
        isClickLeft = Input.GetMouseButton(0);
        isClickRight = Input.GetMouseButton(1);
    }

    void LeftClickItem()
    {
        GameManager.PlaySound(gameManager.audioSource, gameManager.resourcesManager.sounds["itemClick"], false);

        // �� ������ ���� ������ �ٸ��� ������ �θ� ���� Ȯ������ �Ǵ�
        string parentName = transform.parent.parent.parent.parent != null ? transform.parent.parent.parent.parent.name : "";

        bool inventoryToQuick = transform.parent.parent.parent.parent == null && gameManager.panel_inventory.activeSelf;

        if (inventoryToQuick) // �� ���Կ��� �κ��丮
        {
            ChangeItemState("Inventory", "Quick", gameManager.inventory.myItems[type], gameManager.quickSlot.items);
            gameManager.InstallItemOnPanel(gameManager.quickSlot.items, gameManager.quickSlot.panel_items); // ���� �缳��
        }
        else if (parentName.Equals("Chest")) // ���ڿ��� �κ��丮
        {
            ChangeItemState("Inventory", "Chest", gameManager.inventory.myItems[type], gameManager.nowOpenChest.items);
            gameManager.InstallItemOnPanel(gameManager.nowOpenChest.items, gameManager.nowOpenChest.panel_items); // ���� �缳��
        }
        else if(parentName.Equals("Inventory")) // �κ��丮�� �ִ� ������
        {
            if (!gameManager.panel_chest.activeSelf) // �κ��丮���� �� ����
            {
                ChangeItemState("Quick", "Inventory", gameManager.quickSlot.items, gameManager.inventory.myItems[type]);
                gameManager.InstallItemOnPanel(gameManager.quickSlot.items, gameManager.quickSlot.panel_items); // ���� �缳��
            }
            else // �κ��丮���� ����
            {
                ChangeItemState("Chest", "Inventory", gameManager.nowOpenChest.items, gameManager.inventory.myItems[type]);
                gameManager.InstallItemOnPanel(gameManager.nowOpenChest.items, gameManager.nowOpenChest.panel_items); // ���� �缳��
            }
        }
    }

    void RightClickItem()
    {
        // ������ ���ȱ�
        if(gameManager.panel_shop.activeSelf)
        {
            GameManager.PlaySound(gameManager.audioSource, gameManager.resourcesManager.sounds["sell"], false);

            gameManager.SetItem(gameManager.inventory.myItems[type], originalName, "Inventory", -1, false);
            gameManager.InstallItemOnPanel(gameManager.inventory.myItems[type], gameManager.inventory.panel_items);
            gameManager.SetMoney(sellPrice);
        }
    }

    void ChangeItemState(string type, string type2, List<ItemImg> items, List<ItemImg> items2)
    {
        // �������� �ִ� ���������� Ȯ���ϱ� ���� useDurability ������ �� (�������� ���ٸ� �˾Ƽ� üũX)
        ItemImg plusItem = gameManager.SetItem(items, originalName, type, 1, false);

        // �̵� ������ �г��� ������ �ʾ��� ��
        if(plusItem != null)
        {
            Durability minusItem = gameManager.SetItem(items2, originalName, type2, -1, false).GetComponent<Durability>();
            gameManager.inventory.ChangeCategory(this.type); // �κ��丮 �缳��

            //// ������ ������ �ű� �� ������ �ű��
            if (minusItem != null && minusItem.GetComponent<Durability>() != null)
            {
                float remainDura = minusItem.maxUseCount - minusItem.nowUseCount;
                plusItem.GetComponent<Durability>().SetDurability(-remainDura);
            }
        }

    }

    public void InstallOnPanel(Transform target, Transform panel)
    {
        target.transform.SetParent(panel);

        target.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        target.GetComponent<RectTransform>().sizeDelta = panel.GetComponent<RectTransform>().sizeDelta - new Vector2(30, 30);
        target.transform.SetSiblingIndex(0);

        text_count = panel.GetChild(1).GetComponent<TextMeshProUGUI>();
        text_count.gameObject.SetActive(true);
        text_count.GetComponent<TextMeshProUGUI>().text = count.ToString();

        text_count = panel.GetChild(1).GetComponent<TextMeshProUGUI>();
        text_count.gameObject.SetActive(true);
        text_count.GetComponent<TextMeshProUGUI>().text = count.ToString();
    }
}
