using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    // Manager, Data
    GameManager gameManager;
    UIManager uiManager;
    //DataManager dataManager;
    //ShopDatabase shopDatabase;
    //ItemDatabase itemDatabase;

    // Script
    [HideInInspector] public Shop shop;
    private QuickSlotUI quickSlotUI;
    ScrollViewUI scrollViewUI;
    PlayerMovement playerMovement;

    // Item Info
    [SerializeField] public GameObject _itemInfoUI;
    [SerializeField] public TextMeshProUGUI _selectedItemName;
    [SerializeField] public TextMeshProUGUI _selectedItemDescription;
    [HideInInspector] public float itemInfoWidthHalf;
    [HideInInspector] public float itemInfoHeightHalf;
    RectTransform itemInfoRect;

    public ShopSlotUI shopSlotUIPrefab;
    RectTransform slotRect;

    // 초기화
    public void Init()
    {
        this.gameManager = GameManager.Instance;
        this.uiManager = gameManager.UIManager;
        //dataManager = gameManager.DataManager;

        //itemDatabase = dataManager.itemDatabase;
        shop = GetComponent<Shop>();
        quickSlotUI = uiManager.quickSlotUI;

        scrollViewUI = GetComponentInChildren<ScrollViewUI>();
        itemInfoRect = _itemInfoUI.GetComponent<RectTransform>();
        slotRect = shopSlotUIPrefab.GetComponent<RectTransform>();

        scrollViewUI.Init(shopSlotUIPrefab);

        shop.Init();

        playerMovement = shop.player.playerMovement;

        itemInfoWidthHalf = _itemInfoUI.GetComponent<RectTransform>().rect.width / 2;
        itemInfoHeightHalf = _itemInfoUI.GetComponent<RectTransform>().rect.height / 2;

        gameObject.SetActive(false);
    }

    // 상점 ui 활성화
    public void ShopEnable()
    {
        gameObject.SetActive(true);
        quickSlotUI.QuickSlotDisable();
        playerMovement.SwitchPlayerInputAction(true);
    }
    
    // 상점 ui 비활성화
    public void ShopDisable()
    {
        gameObject.SetActive(false);
        quickSlotUI.QuickSlotEnable();
        playerMovement.SwitchPlayerInputAction(false);
    }

    // 설명창 비활성화
    public void InfoHide()
    {
        _itemInfoUI.SetActive(false);
    }

    // 설명창 활성화
    public void InfoShow(ShopSlotUI slot)
    {
        //_itemInfoUI.transform.position = Input.mousePosition + new Vector3(itemInfoWidthHalf, itemInfoHeightHalf);
        _itemInfoUI.transform.position = slot.transform.position;
        itemInfoRect.anchoredPosition += new Vector2(itemInfoWidthHalf + slotRect.sizeDelta.x/2, itemInfoHeightHalf * 1.5f);
        if(itemInfoRect.anchoredPosition.y >= 390)
        {
            itemInfoRect.anchoredPosition = new Vector2(itemInfoRect.anchoredPosition.x, 540 - itemInfoHeightHalf);
        }

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
