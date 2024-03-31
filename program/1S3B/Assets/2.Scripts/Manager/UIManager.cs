using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager
{
    private Player player;

    [Header("Image")]
    public GameObject sleepInfoUI;
    public InventoryUI inventoryUI;

    [Header("EnergyBar")]
    [SerializeField] public GameObject tired;
    [SerializeField] public Slider energyBar;
    private TMP_Text energyText;

    private int maxEnergy;

    private void Start()
    {
        player = gameManager.Player;
        inventoryUI.Init(gameManager, this, player);

        maxEnergy = player.playerMaxEnergy;
        energyText = energyBar.GetComponentInChildren<TMP_Text>();
        EnengyBarSetting();
    }

    private void EnengyBarSetting()
    {
        energyBar.maxValue = maxEnergy;
        energyBar.minValue = 0;

        EnergyBarUpdate(maxEnergy);

        energyText.gameObject.SetActive(false);
        tired.SetActive(false);
    }
    public void EnergyBarUpdate(int playerEnergy)
    {
        if (playerEnergy > maxEnergy)
            playerEnergy = maxEnergy;

        energyBar.value = playerEnergy;
        energyText.text = playerEnergy + "/" + maxEnergy;
    }
    public void TiredIconOnOff(bool value)
    {
        tired.SetActive(value);
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
        if (ui == null)
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
