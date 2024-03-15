using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewUI : MonoBehaviour
{
    public GameObject originItemPrefab;
    private float _itemHeight;
    public List<int> dataList;

    private ScrollRect _scroll;
    private List<GameObject> _itemList;
    private float _offset;

    private void Awake()
    {
        _scroll = GetComponent<ScrollRect>();
        _itemHeight = originItemPrefab.GetComponent<RectTransform>().rect.height + 10;
    }

    private void Start()
    {
        dataList.Clear();
        for(int i = 0; i< 100; i++)
        {
            dataList.Add(i);
        }

        CreateItem();
        SetContentHeight();
    }

    private void Update()
    {
        float contentPositionY = _scroll.content.anchoredPosition.y;
        RectTransform scrollRect = _scroll.GetComponent<RectTransform>();
        float scrollHeight = scrollRect.rect.height;
        foreach (GameObject item in _itemList)
        {
            RelocationItem(item, contentPositionY, scrollHeight);
        }
    }

    private void CreateItem()
    {
        RectTransform scrollRect = _scroll.GetComponent<RectTransform>();
        _itemList = new List<GameObject>();

        int itemCount = (int)(scrollRect.rect.height / _itemHeight) + 2;

        for(int i = 0; i < itemCount; i++)
        {
            GameObject item = Instantiate(originItemPrefab, _scroll.content);
            _itemList.Add(item);

            item.transform.localPosition = new Vector3(0, -i * _itemHeight);
        }

        _offset = _itemList.Count * _itemHeight;
    }

    private void SetContentHeight()
    {
        _scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, dataList.Count * _itemHeight);
    }

    private void RelocationItem(GameObject item, float contentPositionY, float scrollHeight)
    {
        if (item.transform.localPosition.y + contentPositionY > _itemHeight)
        {
            item.transform.localPosition -= new Vector3(0, _offset);
        }
        else if (item.transform.localPosition.y + contentPositionY < -scrollHeight - _itemHeight)
        {
            item.transform.localPosition += new Vector3(0, _offset);
        }
    }
}
