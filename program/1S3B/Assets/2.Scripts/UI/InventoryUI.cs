using System.Collections.Generic;
using TMPro;
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
    InventorySlotUI inventorySlotUI;

    // Item Info
    [SerializeField] public GameObject _itemInfoUI;
    [SerializeField] public TextMeshProUGUI _selectedItemName;
    [SerializeField] public TextMeshProUGUI _selectedItemDescription;
    public float itemInfoWidthHalf;
    public float itemInfoHeightHalf;

    // public ItemSlotUI itemSlotPrefab;
    //public List<ItemSlotUI> uiSlots;

    //[Header("Selected Item")]
    //private InventorySlotUI _selectedItem;
    //private int _selectedItemIndex;

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
        scrollViewUI = GetComponentInChildren<ScrollViewUI>();

        //inventorySlotUIPrefab.inventory = inventory;
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

        itemInfoWidthHalf = _itemInfoUI.GetComponent<RectTransform>().rect.width / 2;
        itemInfoHeightHalf = _itemInfoUI.GetComponent<RectTransform>().rect.height / 2;

        gameObject.SetActive(false);
    }

    // 인벤토리 ui 활성화
    public void InventoryEnable()
    {
        Refresh();
        gameObject.SetActive(!gameObject.activeSelf);
    }

    // 설명창 활성화
    public void InfoShow(InventorySlotUI slot)
    {
        //_itemInfoUI.transform.position = slot.transform.position + new Vector3(itemInfoWidthHalf, itemInfoHeightHalf);
        _itemInfoUI.transform.position = Input.mousePosition + new Vector3(itemInfoWidthHalf, itemInfoHeightHalf);
        _itemInfoUI.SetActive(true);
    }

    // 설명창 비활성화
    public void InfoHide()
    {
        _itemInfoUI.SetActive(false);
    }

    public void Refresh()
    {


        // 전체 슬롯 최신화
        scrollViewUI?.Refresh();


        // 아이템이 10 있다가 8개가 되었다.
        // 끝에 2개가 최신화 가 안됨
        // 이전의 최신화 했던 슬롯의 카운트를 인지
        // 오차범위는 클리어 한다

        // 아이템 기반 최신화
        //for (int i = 0; i < inventory.Items.Count; i++)
        //{
        //    //Item item = inventory.Items[i];

        //    //if (item != null)
        //    //{
        //    //    inventorySlotUIPrefab.Set(item);
        //    //}
        //    //else
        //    //{
        //    //    inventorySlotUIPrefab.Clear();
        //    //}

        //    // 슬롯 셋팅
        //    scrollViewUI.SetSlot(i);
        //}
    }
}