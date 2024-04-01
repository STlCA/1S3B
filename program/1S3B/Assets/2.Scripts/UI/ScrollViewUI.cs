using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 동적 스크롤 스크립트 [뷰?]
// 인벤토리 UI 스크립트에서 슬롯 프리팹 사이즈 가져와서 해당 스크롤뷰 생성 (-> 그럼 인벤토리의 가로 길이도 스크립트에서 관리해야하나? 아님 그냥 에디터에서 해야하나?)
// 스크롤을 움직였을 때 슬롯 재사용
// 높이, 너비 이용하여 각 슬롯에 인덱스 값 부여
// 콜백 - 액션 이용하여 사용된 슬롯 내부의 데이터 파악(-> 근데 내부 내이터 파악을 하려면 반환값이 있어야하는거 아닌감..?)
// 비어있는 슬롯 전달(을 인벤토리 UI에 해줘야하나? ㅋㅋㅋㅋ)
// 제네릭...이 여기에 있어야하나..?
// 인벤토리를 몇 줄로 할지는 스크립트에서 정해야하나 아님 이것도 동적으로 정해야하나?

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewUI : MonoBehaviour
{
    //public GameObject itemSlotPrefab;
    // Script
    InventoryUI inventoryUI;

    // 슬롯 너비, 높이
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

    // 슬롯에 인덱스 부여
    private void SetSlotIndex()
    {

    }

    // 가장 마지막 슬롯 반환

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
