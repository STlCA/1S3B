using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour//, IPointerClickHandler
{
    public QuickSlot quickSlot;
    public QuickSlotUI quickSlotUI;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityTxt;
    private Outline outline;

    public Item item;
    public int index;

    public void Start()
    {
        quickSlot = GameManager.Instance.Player.QuickSlot;
        quickSlotUI = quickSlot.quickSlotUI;
        outline = GetComponent<Outline>();
    }

    public void Set()
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.ItemInfo.SpriteList[0];
        icon.SetNativeSize();
        quantityTxt.text = item.quantity > 1 ? item.quantity.ToString() : string.Empty;
    }

    public void Clear()
    {
        item = null;
        icon.gameObject.SetActive(false);
        quantityTxt.text = string.Empty;
    }

    // 아이템 클릭 했을 때
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (item == null)
    //    {
    //        return;
    //    }

    //    OutlineEnable();
    //    //inventory.UseItem(this, item);
    //}

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
}
