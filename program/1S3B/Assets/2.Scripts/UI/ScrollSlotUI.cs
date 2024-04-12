using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //private ScrollViewUI scrollViewUI;

    //private RectTransform rectTransform;
    //[HideInInspector] public float paddingHeight;
    [SerializeField] private GameObject _itemInfoUI;
    [SerializeField] private TextMeshProUGUI _selectedItemName;
    [SerializeField] private TextMeshProUGUI _selectedItemDescription;

    public int index;

    public virtual void Init()
    {
        //rectTransform = GetComponent<RectTransform>();
        //paddingHeight = 5;
    }
    
    public virtual void Set(int idx)
    {
        index = idx;
    }

    //public virtual void SetSlotSize(out float width, float height)
    //{
    //    height = rectTransform.rect.height + paddingHeight;
    //    width = rectTransform.rect.width;
    //}

    // 아이템에서 마우스를 치웠을 때
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        _itemInfoUI.SetActive(false);
    }

    // 아이템에 마우스를 올렸을 때
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(index);
        // 아이템이 존재하지 않을 때
        //if (GameManager.Instance.UIManager.inventoryUI.slots[index].iteminstance == null)
        //    return;

        //GameManager.Instance.UIManager.inventoryUI.SelectItem(index);
        //itemInfoUI.SetActive(true);
    }

    // 아이템 설명창 업데이트
    public virtual void UpdateItemInfo(string displayName, string description)
    {
        _selectedItemName.text = displayName;
        _selectedItemDescription.text = description;
    }
}
