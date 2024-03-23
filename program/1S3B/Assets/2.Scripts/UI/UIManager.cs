using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject sleepInfoUI;
    public InventoryUI inventoryUI;

    private void Start()
    {
        GameManager.Instance.uIManager = this;
    }

    //ON OFF Change
    public void UIOnOff(GameObject ui)
    {
        if (ui == null)
            return;

        if (ui.activeSelf == true)
            ui.SetActive(false);
        else
            ui.SetActive(true);
    }

    //UION / OFF
    public void UIOn(GameObject ui)
    {
        if (ui == null)
            return;
        ui.SetActive(true);
    }
    public void UIOff(GameObject ui)
    {
        if(ui == null)
            return;

        ui.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void UIOffStopTime(GameObject ui)
    {
        if (ui == null)
            return;

        ui.SetActive(false);
    }
}
