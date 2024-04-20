using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 슬롯 UI 스크립트 [컨트롤러? 프레젠터?]
// 슬롯 초기화
// 슬롯 창 위에서 마우스 움직임 제어 (해당 슬롯에 있는 것 정보 띄우기)

public class InventorySlotUI : ScrollSlotUI, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Inventory inventory;
    public InventoryUI inventoryUI;
    public Image icon;
    public Item _item;
    public TextMeshProUGUI quantityTxt; 
    public GameObject quickApplyTxt;
    private Outline outline;
    //[HideInInspector] public bool QSymbolActive = false;

    #region ScrollSlotUI 오버라이드
    public override void Init()
    {
        base.Init();
        inventory = GameManager.Instance.Player.Inventory;
        inventoryUI = inventory.inventoryUI;
        outline = GetComponent<Outline>();
    }

    public override void SetIndex(int idx)
    {
        base.SetIndex(idx);
        _item = inventory.GetItem(idx);

        if (_item == null)
        {
            // 빈슬롯일 때
            Clear();
        }
        else
        {
            // 아이템이 들어있을 때
            Load(idx);
        }


        // item ������ ����
    }

    // 아이템에 마우스를 올렸을 때
    //public override void OnPointerEnter(PointerEventData eventData)
    //{
    //    // 아이템이 존재하지 않을 때
    //    if (_item == null)
    //    {
    //        Debug.Log("null");
    //        return;
    //    }
    //    Debug.Log(_item.ItemInfo.Name);

    //    inventory.SelectItem(this);
    //    _itemInfoUI.SetActive(true);
    //}
    #endregion // ScrollSlotUI 오버라이드

    //[SerializeField] private GameObject _itemInfoUI;
    //[SerializeField] private TextMeshProUGUI _selectedItemName;
    //[SerializeField] private TextMeshProUGUI _selectedItemDescription;

    //public Image icon;
    //private Item _item;

    //public int index;

    // 슬롯 창 불러오기
    public void Load(int idx)
    {
        Item item = inventory.Items[idx];
        Set(item);
    }

    // 슬롯 창 설정 초기화
    public void Set(Item item)
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.ItemInfo.SpriteList[0]; // ***** TODO : 사용할 스프라이트 인덱스 확인하기!!!
        quantityTxt.text = item.quantity > 1 ? item.quantity.ToString() : string.Empty;
        SymbolSetActive();
    }

    // 슬롯 창 초기화
    public void Clear()
    {
        _item = null;
        icon.gameObject.SetActive(false);
        quantityTxt.text = string.Empty;
        //quickApplyTxt.SetActive(false);
        SymbolSetActive();
    }

    // 아이템에서 마우스를 치웠을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        //inventoryUI._itemInfoUI.SetActive(false);
        inventoryUI.InfoHide();
    }

    // 아이템에 마우스를 올렸을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 아이템이 존재하지 않을 때
        if (_item == null)
        {
            Debug.Log("null");
            return;
        }
        Debug.Log(_item.ItemInfo.Name);

        inventory.SelectItem(this);
        //inventoryUI._itemInfoUI.transform.position = Input.mousePosition;
        //inventoryUI._itemInfoUI.SetActive(true);
        inventoryUI.InfoShow(this);

        // 아이템이 존재하지 않을 때
        //if (GameManager.Instance.UIManager.inventoryUI.slots[index].iteminstance == null)
        //    return;

        //GameManager.Instance.UIManager.inventoryUI.SelectItem(index);
        //itemInfoUI.SetActive(true);
    }

    // 아이템 설명창 업데이트
    public void UpdateItemInfo(string displayName, string description)
    {
        inventoryUI._selectedItemName.text = displayName;
        inventoryUI._selectedItemDescription.text = description;
    }

    // 아이템 클릭했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        if(_item == null)
        {
            return;
        }

        OutlineEnable();
        //inventory.UseItem(this, _item);
        //inventory.OnClickButtonQuickApply(this, _item);
        inventory.SelectSlot(this);

        //if (eventData.button == PointerEventData.InputButton.Left)
        //{
        //    inventory.UseItem(this, _item);
        //}
        //else if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    inventory.InputQuickSlot(this, _item);
        //}
    }

    // 아이템 선택
    public void OutlineEnable()
    {
        outline.enabled = true;
    }

    // 아이템 선택 해제
    public void OutlineDisable()
    {
        outline.enabled = false;
    }

    // 아이템이 퀵 슬롯에 들어있는지 나타내는 마크 활성/비활성
    public void SymbolSetActive()
    {
        if(_item == null)
        {
            quickApplyTxt.SetActive(false);
            return;
        }

        quickApplyTxt.SetActive(_item.QSymbolActive);
    }
}