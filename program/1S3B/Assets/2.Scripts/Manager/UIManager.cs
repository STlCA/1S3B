using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class UIManager : Manager
{
    private Camera main;
    private Player player;

    [Header("Image")]
    public GameObject sleepInfoUI;
    public InventoryUI inventoryUI;
    public GameObject equipIcon;

    [Header("EnergyBar")]
    [SerializeField] private GameObject tired;
    [SerializeField] private Slider energyBar;
    private TMP_Text energyText;

    [Header("MiniMap")]
    public Image miniMap;
    public GameObject playerObj;//왜인지 RectTransform으로 직접할당하면 안됨
    private RectTransform playerImage;

    private int maxEnergy;

    private void Start()
    {
        main = Camera.main;

        player = gameManager.Player;
        inventoryUI.Init(gameManager, this, player);

        maxEnergy = player.playerMaxEnergy;
        energyText = energyBar.GetComponentInChildren<TMP_Text>();
        EnengyBarSetting();

        playerImage = playerObj.GetComponent<RectTransform>();
    }

    public void MiniMapPosition(PlayerMap map)
    {
        player.playerMap = map;

        switch (map)
        {
            case PlayerMap.Farm:
                playerImage.anchoredPosition = new Vector2(528, 186);    
                break;                                   
            case PlayerMap.FarmRoad:                     
                playerImage.anchoredPosition = new Vector2(29, 359);
                break;                                   
            case PlayerMap.Town:                         
                playerImage.anchoredPosition = new Vector2(50, 134);
                break;                                   
            case PlayerMap.TownRoad:                     
                playerImage.anchoredPosition = new Vector2(267, 134);
                break;                                   
            case PlayerMap.Forest:                       
                playerImage.anchoredPosition = new Vector2(-501, 151);
                break;                                   
            case PlayerMap.ForestRoad:                   
                playerImage.anchoredPosition = new Vector2(-176, 138);
                break;                                   
            case PlayerMap.Beach:                        
                playerImage.anchoredPosition = new Vector2(37, -129);
                break;                                   
            case PlayerMap.BeachR:                       
                playerImage.anchoredPosition = new Vector2(375, -104);
                break;                                   
            case PlayerMap.BeachL:                       
                playerImage.anchoredPosition = new Vector2(-488, -167);
                break;                                   
            case PlayerMap.BossRoom:                     
                playerImage.anchoredPosition = new Vector2(-701, 96);
                break;                                   
            case PlayerMap.Island:                       
                playerImage.anchoredPosition = new Vector2(142, -384);
                break;                                   
            case PlayerMap.Quarry:                       
                playerImage.anchoredPosition = new Vector2(693, -296);
                break;

        }
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
