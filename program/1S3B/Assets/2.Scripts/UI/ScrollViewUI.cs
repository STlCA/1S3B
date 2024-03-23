using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewUI : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    private float _itemHeight;
    private float _itemWidth;

    private ScrollRect _scroll;
    private RectTransform _scrollRect;
    private List<GameObject> _itemList;
    private float _offset;

    private void Awake()
    {
        _scroll = GetComponent<ScrollRect>();
        _scrollRect = _scroll.GetComponent<RectTransform>();
        _itemHeight = itemSlotPrefab.GetComponent<RectTransform>().rect.height + 10;
        _itemWidth = itemSlotPrefab.GetComponent<RectTransform>().rect.width;
    }

    private void Start()
    {
        CreateSlots();
        SetContentHeight();
    }

    // Slot 생성
    private void CreateSlots()
    {
        _itemList = new List<GameObject>();

        int itemCount = ((int)(_scrollRect.rect.height / _itemHeight) + 3) * (int)(_scrollRect.rect.width / _itemWidth);

        for (int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(itemSlotPrefab, _scroll.content);
            _itemList.Add(item);

            item.transform.localPosition = new Vector3(0, -i * _itemHeight);
        }

        _offset = _itemList.Count / (int)(_scrollRect.rect.width / _itemWidth) * _itemHeight;
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
        foreach (GameObject item in _itemList)
        {
            bool isChanged = RelocationSlot(item, contentPositionY, scrollHeight);
            if(isChanged)
            {
                int idx = (int)(-item.transform.localPosition.y / _itemHeight);
            }
        }
    }

    // Slot 재사용
    private bool RelocationSlot(GameObject item, float contentPositionY, float scrollHeight)
    {
        if (item.transform.localPosition.y + contentPositionY > _itemHeight)
        {
            item.transform.localPosition -= new Vector3(0, _offset);
            return true;
        }
        else if (item.transform.localPosition.y + contentPositionY < -scrollHeight - _itemHeight)
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
