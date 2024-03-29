using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpInvenBtn : MonoBehaviour
{
    [SerializeField] private GameObject inventory;

    public void OnClickInvenBtn()
    {
        inventory.SetActive(!inventory.activeSelf);
    }

    public void OnClickAddItem()
    {
        GameManager.Instance.uIManager.inventoryUI.AddItem(GameManager.Instance.dataManager.itemDatabase.ItemData[Random.Range(0, GameManager.Instance.dataManager.itemDatabase.ItemData.Count)]);
    }
}
