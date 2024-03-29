using System.Collections.Generic;
using UnityEngine;

//public class ItemSlot  
//{
//    public Item item;
//    public int quantity;
//}

public class ItemSlot
{
    public ItemInstance iteminstance;
    public int quantity;
}

public class InventoryUI : MonoBehaviour
{
    GameManager gameManager;
    DataManager dataManager;
    ItemDatabase itemDatabase;
    ScrollViewUI scrollViewUI;

    //List<ItemInstance> hasItems = new List<ItemInstance>();

    //public ItemSlotUI[] uiSlots;
    public ItemSlotUI itemSlotPrefab;
    public List<ItemSlotUI> uiSlots;
    public ItemSlot[] slots;

    [Header("Selected Item")]
    private ItemSlot _selectedItem;
    private int _selectedItemIndex;

    void Awake()
    {

    }

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
    public void Init()
    {
        gameManager = GameManager.Instance;
        dataManager = gameManager.dataManager;

        itemDatabase = dataManager.itemDatabase;
        scrollViewUI = GetComponent<ScrollViewUI>();

        // 슬롯 초기화
        //uiSlots = new ItemSlotUI[uiSlots.Count];
        slots = new ItemSlot[uiSlots.Count];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }

        gameObject.SetActive(false);
    }

    // 인벤토리에 아이템 추가
    public void AddItem(Item item)
    {
        // 아이템이 도구가 아닐 때
        if (item.canStack)
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if (slotToStackTo != null)
            {
                slotToStackTo.quantity++;
                UpdateUI();
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();

        if (emptySlot != null)
        {
            emptySlot.iteminstance.item = item;
            emptySlot.quantity = 1;
            UpdateUI();
            return;
        }
    }

    // 획득한 아이템이 기존에 획득했것인지 확인
    ItemSlot GetItemStack(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].iteminstance.item == item && slots[i].quantity < item.Stack)
            {
                return slots[i];
            }
        }

        return null;
    }

    // 비어있는 슬롯 확인
    public ItemSlot GetEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].iteminstance.item == null)
                return slots[i];
        }

        return null;
    }

    // UI 업데이트
    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].iteminstance.item != null)
            {
                uiSlots[i].Set(slots[i]);
            }
            else
            {
                uiSlots[i].Clear();
            }
        }
    }

    // 아이템 선택
    public void SelectItem(int index)
    {
        _selectedItem = slots[index];
        _selectedItemIndex = index;
        string infoString = "";

        for(int i = 0; i < _selectedItem.iteminstance.item.Description.Count; i++)
        {
            infoString += _selectedItem.iteminstance.item.Description[i];
        }

        uiSlots[index].UpdateItemInfo(_selectedItem.iteminstance.item.Name, infoString);
    }

    // 아이템 제거
    private void RemoveSelectedItem()
    {
        _selectedItem.quantity--;

        if (_selectedItem.quantity <= 0)
        {
            _selectedItem.iteminstance.item = null;
        }

        UpdateUI();
    }
}