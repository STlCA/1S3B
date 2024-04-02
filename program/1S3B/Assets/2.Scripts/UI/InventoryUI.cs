using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 인벤토리 UI 스크립트 [컨트롤러? 프레젠터?]
// 슬롯 UI 스크립트로 슬롯 프리팹 선언 (-> 그럼 상점용 슬롯이 필요하면 따로 또 선언해줘야함?)
// 인벤토리 Refresh
// 쓰려고 하는 아이템을 여기서 처리해줘야하나?

//public class ItemSlot  
//{
//    public Item item;
//    public int quantity;
//}

public class InventoryUI : MonoBehaviour
{
    // Manager, Data
    GameManager gameManager;
    UIManager uiManager;
    DataManager dataManager;
    ItemDatabase itemDatabase;

    // Script
    Player player;
    Inventory inventory;
    ScrollViewUI scrollViewUI;

    // public ItemSlotUI itemSlotPrefab;
    //public List<ItemSlotUI> uiSlots;

    [Header("Selected Item")]
    private InventorySlotUI _selectedItem;
    private int _selectedItemIndex;

    public InventorySlotUI inventorySlotUIPrefab;

    private void Start()
    {
        //Init();

        //uiSlots = new ItemSlotUI[scrollViewUI._itemList.Count];
        //slots = new ItemSlot[uiSlots.Length];

        //for (int i = 0; i < slots.Length; i++)
        //{
        //    slots[i] = new ItemSlot();
        //    uiSlots[i].index = i;
        //    uiSlots[i].Clear();
        //}

        //gameObject.SetActive(false);
    }

    // 초기화
    public void Init(GameManager gameManager, UIManager uiManager, Player player)
    {
        this.player = player;
        inventory = player.Inventory;

        this.gameManager = gameManager;
        this.uiManager = uiManager;
        dataManager = gameManager.DataManager;

        itemDatabase = dataManager.itemDatabase;
        scrollViewUI = GetComponent<ScrollViewUI>();

        inventorySlotUIPrefab.inventory = inventory;
        scrollViewUI.Init(inventorySlotUIPrefab); // TODO
        //// 슬롯 초기화
        ////uiSlots = new ItemSlotUI[uiSlots.Count];
        //slots = new ItemSlot[uiSlots.Count];

        //for (int i = 0; i < slots.Length; i++)
        //{
        //    slots[i] = new ItemSlot();
        //    uiSlots[i].index = i;
        //    uiSlots[i].Clear();
        //}

        // scrollViewUI.Init(50);

        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        for (int i = 0; i < inventory.Items.Count; i++)
        {
            Item item = inventory.Items[i];
            // 슬롯 셋팅
            // scrollViewUI.SetSlot(i, item);
        }

        //for (int i = 0; i < slots.Length; i++) 
        //{
        //    if (slots[i].iteminstance.item != null)
        //    {
        //        uiSlots[i].Set(slots[i]);
        //    }
        //    else
        //    {
        //        uiSlots[i].Clear();
        //    }
        //}
    }

    //// 인벤토리에 아이템 추가
    //public void AddItem(ItemInfo item)
    //{
    //    // 아이템이 도구가 아닐 때
    //    if (item.canStack)
    //    {
    //        ItemSlot slotToStackTo = GetItemStack(item);
    //        if (slotToStackTo != null)
    //        {
    //            slotToStackTo.quantity++;
    //            UpdateUI();
    //            return;
    //        }
    //    }

    //    ItemSlot emptySlot = GetEmptySlot();

    //    if (emptySlot != null)
    //    {
    //        emptySlot.iteminstance.item = item;
    //        emptySlot.quantity = 1;
    //        UpdateUI();
    //        return;
    //    }
    //}

    //// 획득한 아이템이 기존에 획득했것인지 확인
    //ItemSlot GetItemStack(ItemInfo item)
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].iteminstance.item == item && slots[i].quantity < item.Stack)
    //        {
    //            return slots[i];
    //        }
    //    }

    //    return null;
    //}

    //// 비어있는 슬롯 확인
    //public ItemSlot GetEmptySlot()
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].iteminstance.item == null)
    //            return slots[i];
    //    }

    //    return null;
    //}

    //// UI 업데이트
    //void UpdateUI()
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (slots[i].iteminstance.item != null)
    //        {
    //            uiSlots[i].Set(slots[i]);
    //        }
    //        else
    //        {
    //            uiSlots[i].Clear();
    //        }
    //    }
    //}

    //// 아이템 선택
    //public void SelectItem(int index)
    //{
    //    _selectedItem = slots[index];
    //    _selectedItemIndex = index;
    //    string infoString = "";

    //    for (int i = 0; i < _selectedItem.iteminstance.item.Description.Count; i++)
    //    {
    //        infoString += _selectedItem.iteminstance.item.Description[i];
    //    }

    //    uiSlots[index].UpdateItemInfo(_selectedItem.iteminstance.item.Name, infoString);
    //}

    //// 아이템 제거
    //private void RemoveSelectedItem()
    //{
    //    _selectedItem.quantity--;

    //    if (_selectedItem.quantity <= 0)
    //    {
    //        _selectedItem.iteminstance.item = null;
    //    }

    //    UpdateUI();
    //}
}