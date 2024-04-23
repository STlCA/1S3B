using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class Shop : MonoBehaviour
{
    // Manager
    GameManager gameManager;
    DataManager dataManager;
    DayCycleHandler dayCycleHandler;
    ShopDatabase shopDatabase;
    ItemDatabase itemDatabase;
    //UIManager uiManager;

    // Script
    [HideInInspector] public ShopUI shopUI;

    [SerializeField] List<ItemInfo> items = new();
    public List<ItemInfo> Items { get { return items; } }

    public Dictionary<Season, ShopInfo> dicShopInfo = new();

    private bool isRefresh;
    private Season season;

    [Header("Selected Item")] // 마우스가 가리키는 아이템
    private ShopSlotUI _selectedItem;
    private int _selectedItemIndex;

    ShopSlotUI selectedSlotUI; // 클릭한 아이템

    public void Init()
    {
        gameManager = GameManager.Instance;
        dataManager = gameManager.DataManager;
        shopUI = GetComponent<ShopUI>();

        dayCycleHandler = gameManager.DayCycleHandler;
        dayCycleHandler.changeSeasonAction += ChangeSeason;

        shopDatabase = dataManager.shopDatabase;
        itemDatabase = dataManager.itemDatabase;

        isRefresh = true;
        season = dayCycleHandler.currentSeason;

        dicShopInfo[Season.Spring] = shopDatabase.GetShopByKey(1);
        dicShopInfo[Season.Summer] = shopDatabase.GetShopByKey(2);
        dicShopInfo[Season.Fall] = shopDatabase.GetShopByKey(3);
        dicShopInfo[Season.Winter] = shopDatabase.GetShopByKey(4);

        ChangeSeason(season);

        OpenShop(); // 나중에 
    }

    // 상점 열기
    public void OpenShop()
    {
        if (isRefresh)
        {
            isRefresh = false;
            SetSeasonShop();
        }

        // 상점을 열어요~~
        shopUI.InventoryEnable();
    }

    // 시즌별 상품 최신화
    private void SetSeasonShop()
    {
        shopUI.Refresh(items.Count);
    }

    // 계절이 바뀌었을 때
    public void ChangeSeason(Season season)
    {
        isRefresh = true;
        this.season = season;

        ShopInfo shopInfo = dicShopInfo[season];
        items.Clear();

        for (int i = 0; i < shopInfo.Items.Count; i++)
        {
            items.Add(itemDatabase.GetItemByKey(shopInfo.Items[i]));
        }
        SetSeasonShop();
    }

    // 아이템 선택
    public ItemInfo GetItem(int idx)
    {
        // 예외처리
        if (items.Count <= idx)
        {
            return null;
        }

        return items[idx];
    }

    // 아이템 선택
    public void SelectItem(ShopSlotUI item)
    {
        _selectedItem = item;
        _selectedItemIndex = item.index;

        string infoString = "";

        for (int i = 0; i < _selectedItem.item.Description.Count; i++)
        {
            infoString += _selectedItem.item.Description[i];
        }

        _selectedItem.UpdateItemInfo(_selectedItem.item.Name, infoString);
    }

    public void BuyItem(ShopSlotUI slot)
    {
        selectedSlotUI = slot;
    }
}