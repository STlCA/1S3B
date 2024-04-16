using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 동적 스크롤 스크립트 [뷰?]
// 인벤토리 UI 스크립트에서 슬롯 프리팹 사이즈 가져와서 해당 스크롤뷰 생성 (-> 그럼 인벤토리의 가로 길이도 스크립트에서 관리해야하나? 아님 그냥 에디터에서 해야하나?)
// 콜백 - 액션 이용하여 사용된 슬롯 내부의 데이터 파악(-> 근데 내부 내이터 파악을 하려면 반환값이 있어야하는거 아닌감..?)
// 인벤토리를 몇 줄로 할지는 스크립트에서 정해야하나 아님 이것도 동적으로 정해야하나?

[RequireComponent(typeof(ScrollRect))]
public class ScrollViewUI : MonoBehaviour
{
    //public GameObject itemSlotPrefab;
    // Script
    InventoryUI inventoryUI;

    //// 슬롯 너비, 높이
    //private float _itemHeight;
    //private float _itemWidth;

    private ScrollRect _scroll;
    private RectTransform _scrollRect;
    //public List<GameObject> _itemList;
    private float _offset;

    private List<ScrollSlotUI> uiSlots;

    private ScrollSlotUI slotPrefab;
    private float _slotPrefabHeight;
    private float _slotPrefabWidth;
    private float _padding = 5;

    // 스크롤뷰 초기화
    public void Init(ScrollSlotUI slot)
    {
        slotPrefab = slot;

        inventoryUI = GetComponent<InventoryUI>();
        _scroll = GetComponent<ScrollRect>();
        _scrollRect = _scroll.GetComponent<RectTransform>();
        //_itemHeight = slotPrefab.GetComponent<RectTransform>().rect.height + 5;
        //_itemWidth = slotPrefab.GetComponent<RectTransform>().rect.width;

        //slotPrefab.SetSlotSize(out _slotPrefabWidth, _slotPrefabHeight);
        // 슬롯 프리팹의 크기 설정
        RectTransform rectTransform = slotPrefab.GetComponent<RectTransform>();
        _slotPrefabWidth = rectTransform.rect.width + _padding;
        _slotPrefabHeight = rectTransform.rect.height + _padding;

        CreateSlots();
        SetContentHeight();
    }

    // Slot 생성
    private void CreateSlots()
    {
        uiSlots = new List<ScrollSlotUI>();

        int itemCount = ((int)(_scrollRect.rect.height / _slotPrefabHeight) + 3) * (int)(_scrollRect.rect.width / _slotPrefabWidth);

        for (int i = 0; i < itemCount; i++)
        {
            ScrollSlotUI item = Instantiate(slotPrefab, _scroll.content);
            item.Init();
            uiSlots.Add(item);

            item.transform.localPosition = new Vector3(0, -i * _slotPrefabHeight);
            SetIndex(item, i);
        }

        _offset = uiSlots.Count / (int)(_scrollRect.rect.width / _slotPrefabWidth) * _slotPrefabHeight;
    }

    // 전체 컨텐츠의 길이 세팅
    private void SetContentHeight()
    {
        _scroll.content.sizeDelta = new Vector2(_scroll.content.sizeDelta.x, _slotPrefabHeight * 10); // y 값 data 길이에 따른 변수로 바꾸기!
    }

    private void Update()
    {
        float contentPositionY = _scroll.content.anchoredPosition.y;
        float scrollHeight = _scrollRect.rect.height;
        foreach (ScrollSlotUI slot in uiSlots)
        {
            bool isChanged = RelocationSlot(slot, contentPositionY, scrollHeight);
            // 슬롯 인덱스 설정
            if(isChanged)
            {
                // int idx = (int)(-item.transform.localPosition.y / _slotPrefabHeight);

                int y = (int)(-slot.transform.localPosition.y / (_slotPrefabHeight + _padding));
                int x = (int)(slot.transform.localPosition.x / (_slotPrefabWidth + _padding));

                int idx = y * (int)(_scrollRect.rect.width / _slotPrefabWidth) + x;

                SetIndex(slot, idx);
            }
        }
    }

    // Slot 재사용
    private bool RelocationSlot(ScrollSlotUI slot, float contentPositionY, float scrollHeight)
    {
        if (slot.transform.localPosition.y + contentPositionY > _slotPrefabHeight * 1.5)
        {
            slot.transform.localPosition -= new Vector3(0, _offset);
            return true;
        }
        else if (slot.transform.localPosition.y + contentPositionY < -scrollHeight - _slotPrefabHeight * 1.5)
        {
            slot.transform.localPosition += new Vector3(0, _offset);
            //item.Set(0);     // TODO  idx 던져주기
            return true;
        }
        return false;
    }

    // 슬롯 인덱스 값 설정
    private void SetIndex(ScrollSlotUI slot, int idx)
    {
        slot.SetIndex(idx);
    }

    // 가장 마지막 슬롯 반환
    public void ReturnEndSlot()
    {

    }

    public void SetSlot(int i)
    {

        uiSlots[i].SetIndex(i);
    }


    public void Refresh()
    {
        for(int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].SetIndex(i);
        }
    }

    //private void SetData(ScrollSlotUI item, int idx)
    //{
    //    if (idx < 0 || idx >= dataList.Count)
    //    {
    //        item.gameObject.SetActive(false);
    //        return;
    //    }
    //    item.gameObject.SetActive(true);
    //}
}
