using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewUI : MonoBehaviour
{
    //public GameObject itemSlotPrefab;
    InventoryUI inventoryUI;
    private float _itemHeight;
    private float _itemWidth;

    private ScrollRect _scroll;
    private RectTransform _scrollRect;
    //public List<GameObject> _itemList;
    private float _offset;

    private void Awake()
    {
        inventoryUI = GetComponent<InventoryUI>();
        _scroll = GetComponent<ScrollRect>();
        _scrollRect = _scroll.GetComponent<RectTransform>();
        _itemHeight = inventoryUI.itemSlotPrefab.GetComponent<RectTransform>().rect.height + 5;
        _itemWidth = inventoryUI.itemSlotPrefab.GetComponent<RectTransform>().rect.width;
    }

    private void Start()
    {
        CreateSlots();
        SetContentHeight();

        GameManager.Instance.uIManager.inventoryUI.Init();
    }

    // Slot 생성
    private void CreateSlots()
    {
        inventoryUI.uiSlots = new List<ItemSlotUI>();

        int itemCount = ((int)(_scrollRect.rect.height / _itemHeight) + 3) * (int)(_scrollRect.rect.width / _itemWidth);

        for (int i = 0; i < itemCount; i++)
        {
            ItemSlotUI item = Instantiate(inventoryUI.itemSlotPrefab, _scroll.content);
            inventoryUI.uiSlots.Add(item);

            item.transform.localPosition = new Vector3(0, -i * _itemHeight);
        }

        _offset = inventoryUI.uiSlots.Count / (int)(_scrollRect.rect.width / _itemWidth) * _itemHeight;
    }

    // 전체 컨텐츠의 길이 세팅
    private void SetContentHeight()
    {
        _scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, _itemHeight * 10); // y 값 data 길이에 따른 변수로 바꾸기!
    }

    private void Update()
    {
        float contentPositionY = _scroll.content.anchoredPosition.y;
        float scrollHeight = _scrollRect.rect.height;
        foreach (ItemSlotUI item in inventoryUI.uiSlots)
        {
            bool isChanged = RelocationSlot(item, contentPositionY, scrollHeight);
            if(isChanged)
            {
                int idx = (int)(-item.transform.localPosition.y / _itemHeight);
            }
        }
    }

    // Slot 재사용
    private bool RelocationSlot(ItemSlotUI item, float contentPositionY, float scrollHeight)
    {
        if (item.transform.localPosition.y + contentPositionY > _itemHeight*1.5)
        {
            item.transform.localPosition -= new Vector3(0, _offset);
            return true;
        }
        else if (item.transform.localPosition.y + contentPositionY < -scrollHeight - _itemHeight*1.5)
        {
            item.transform.localPosition += new Vector3(0, _offset);
            return true;
        }
        return false;
    }

    //private void SetData(GameObject item, int idx)
    //{
    //    if (idx < 0 || idx >= dataList.Count)
    //    {
    //        item.gameObject.SetActive(false);
    //        return;
    //    }
    //    item.gameObject.SetActive(true);
    //}
}
