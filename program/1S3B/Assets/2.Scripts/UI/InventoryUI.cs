using System.Collections.Generic;
using UnityEngine;

public class ItemSlot  
{
    public Item item;
    public int quantity;
}

public class InventoryUI : MonoBehaviour
{
    GameManager gameManager;
    DataManager dataManager;
    ItemDatabase itemDatabase;
    CropDatabase cropDatabase;

    List<ItemInstance> hasItems = new List<ItemInstance>();
    List<CropInstance> hasCrops = new List<CropInstance>();

    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;

    void Awake()
    {

    }

    private void Start()
    {
        Init();

        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }

        gameObject.SetActive(false);
    }

    // 초기화
    private void Init()
    {
        gameManager = GameManager.Instance;
        dataManager = gameManager.dataManager;

        itemDatabase = dataManager.itemDatabase;
        cropDatabase = dataManager.cropDatabase;
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
            emptySlot.item = item;
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
            if (slots[i].item == item && slots[i].quantity < item.Stack)
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
            if (slots[i].item == null)
                return slots[i];
        }

        return null;
    }

    // UI 업데이트
    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
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
        selectedItem = slots[index];
        selectedItemIndex = index;
        string infoString = "";

        for(int i = 0; i < selectedItem.item.Description.Count; i++)
        {
            infoString += selectedItem.item.Description[i];
        }

        uiSlots[index].UpdateItemInfo(selectedItem.item.Name, infoString);
    }

    // 아이템 제거
    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if (selectedItem.quantity <= 0)
        {
            selectedItem.item = null;
        }

        UpdateUI();
    }
}