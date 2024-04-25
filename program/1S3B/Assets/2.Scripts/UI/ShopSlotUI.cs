using Constants;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotUI : ScrollSlotUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector] public Shop shop;
    [HideInInspector] public ShopUI shopUI;
    [HideInInspector] public ShopInfo shopInfo;
    [HideInInspector] public ItemInfo item;
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemCost;

    #region ScrollSlotUI 오버라이드
    public override void Init()
    {
        base.Init();
        shopUI = GameManager.Instance.UIManager.shopUI;
        shop = shopUI.shop;
    }

    public override void SetIndex(int idx)
    {
        base.SetIndex(idx);
        item = shop.GetItem(idx);

        if (item == null)
        {
            // 빈슬롯일 때
            Clear();
            gameObject.SetActive(false);
        }
        else
        {
            // 아이템이 들어있을 때
            Load(idx);
            gameObject.SetActive(true);
        }
    }
    #endregion // ScrollSlotUI 오버라이드

    // 슬롯 창 불러오기
    public void Load(int idx)
    {
        ItemInfo item = shop.Items[idx];
        Set(item);
    }

    // 슬롯 창 설정 초기화
    public void Set(ItemInfo item)
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.SpriteList[0];
        itemName.text = item.Name;
        itemCost.text = item.BuyGold.ToString();
        icon.SetNativeSize();
    }

    // 슬롯 창 초기화
    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
    }

    // 아이템에서 마우스를 치웠을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        shopUI.InfoHide();
    }

    // 아이템에 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템이 존재하지 않을 때
        if (item == null)
        {
            Debug.Log("null");
            return;
        }
        Debug.Log(item.Name);

        shop.SelectItem(this);
        shopUI.InfoShow(this);
    }

    // 아이템 설명창 업데이트
    public void UpdateItemInfo(string displayName, string description)
    {
        shopUI._selectedItemName.text = displayName;
        shopUI._selectedItemDescription.text = description;
    }

    // 아이템 클릭했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        GameManager.Instance.SoundManager.GameAudioClipPlay((int)MainAudioClip.Sell);
        shop.BuyItem(this);
    }
}
