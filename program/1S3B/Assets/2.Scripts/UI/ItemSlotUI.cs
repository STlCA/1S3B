using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 슬롯 UI 스크립트 [컨트롤러? 프레젠터?]
// 슬롯 초기화
// 슬롯 창 위에서 마우스 움직임 제어 (해당 슬롯에 있는 것 정보 띄우기)


public class ItemSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _itemInfoUI;
    [SerializeField] private TextMeshProUGUI _selectedItemName;
    [SerializeField] private TextMeshProUGUI _selectedItemDescription;

    public Image icon;
    private Item _item;

    public int index;

    // 슬롯 창 설정 초기화
    public void Set(Item item)
    {
        this._item = item;
        icon.gameObject.SetActive(true);
        icon.sprite = item.ItemInfo.SpriteList[0];        
    }

    // 슬롯 창 초기화
    public void Clear()
    {
        _item = null;
        icon.gameObject.SetActive(false);
    }

    // 아이템에서 마우스를 치웠을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        _itemInfoUI.SetActive(false);
    }

    // 아이템에 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템이 존재하지 않을 때
        //if (GameManager.Instance.UIManager.inventoryUI.slots[index].iteminstance == null)
        //    return;

        //GameManager.Instance.UIManager.inventoryUI.SelectItem(index);
        //itemInfoUI.SetActive(true);
    }

    // 아이템 설명창 업데이트
    public void UpdateItemInfo(string displayName, string description)
    {
        _selectedItemName.text = displayName;
        _selectedItemDescription.text = description;
    }
}