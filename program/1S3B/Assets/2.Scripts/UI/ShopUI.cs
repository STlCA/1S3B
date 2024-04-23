using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    // Manager, Data
    //GameManager gameManager;
    //UIManager uiManager;
    //DataManager dataManager;
    //ShopDatabase shopDatabase;
    //ItemDatabase itemDatabase;

    // Script
    [HideInInspector] public Shop shop;
    ScrollViewUI scrollViewUI;

    // Item Info
    [SerializeField] public GameObject _itemInfoUI;
    [SerializeField] public TextMeshProUGUI _selectedItemName;
    [SerializeField] public TextMeshProUGUI _selectedItemDescription;
    [HideInInspector] public float itemInfoWidthHalf;
    [HideInInspector] public float itemInfoHeightHalf;

    public ShopSlotUI shopSlotUIPrefab;

    // 초기화
    public void Init()
    {
        //this.gameManager = gameManager;
        //this.uiManager = uiManager;
        //dataManager = gameManager.DataManager;

        //itemDatabase = dataManager.itemDatabase;
        shop = GetComponent<Shop>();

        scrollViewUI = GetComponentInChildren<ScrollViewUI>();

        scrollViewUI.Init(shopSlotUIPrefab);

        shop.Init();

        itemInfoWidthHalf = _itemInfoUI.GetComponent<RectTransform>().rect.width / 2;
        itemInfoHeightHalf = _itemInfoUI.GetComponent<RectTransform>().rect.height / 2;

        gameObject.SetActive(false);
    }

    // 상점 ui 활성화/비활성화
    public void InventoryEnable()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    // 설명창 비활성화
    public void InfoHide()
    {
        _itemInfoUI.SetActive(false);
    }

    // 설명창 활성화
    public void InfoShow(ShopSlotUI slot)
    {
        _itemInfoUI.transform.position = Input.mousePosition + new Vector3(itemInfoWidthHalf, itemInfoHeightHalf);
        _itemInfoUI.SetActive(true);
    }

    // 상점 최신화
    public void Refresh(int count)
    {
        scrollViewUI?.SetContentHeight(count);
        // 전체 슬롯 최신화
        scrollViewUI?.Refresh();
    }
}
