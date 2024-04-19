using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantity;

    public void Set(Item item)
    {
        quantity.text = item.quantity > 1 ? item.quantity.ToString() : string.Empty;
    }

    public void Clear()
    {
        quantity.text = string.Empty;
    }
}
