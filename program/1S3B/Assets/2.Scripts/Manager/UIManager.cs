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
    private Player player;

    [Header("Image")]
    public GameObject sleepInfoUI;
    public QuickSlotUI quickSlotUI;
    public InventoryUI inventoryUI;
    public ShopUI shopUI;
    public GameObject keepOutInfo;

    [Header("EnergyBar")]
    [SerializeField] private GameObject tired;
    [SerializeField] private Slider energyBar;
    private TMP_Text energyText;

    [Header("MiniMap")]
    public Image miniMap;
    public GameObject playerObj;//������ RectTransform���� �����Ҵ��ϸ� �ȵ�
    private RectTransform playerImage;

    private int maxEnergy;

    private void Start()
    {
        player = gameManager.Player;

        maxEnergy = player.playerMaxEnergy;
        energyText = energyBar.GetComponentInChildren<TMP_Text>();
        EnengyBarSetting();

        playerImage = playerObj.GetComponent<RectTransform>();
        playerImage.anchoredPosition = new Vector2(528, 186);//�����Ҷ� ��ġ ���߿� �����ؼ�

        quickSlotUI.Init(gameManager, this, player);
        inventoryUI.Init(gameManager, this, player);
        shopUI.Init();
    }

    //public void EquipIconChange(PlayerEquipmentType type)
    //{
    //    equipIcon.sprite = libraryAsset.GetSprite("Equip", type.ToString());
    //}
    //public void EquipIconChange(string type)
    //{
    //    equipIcon.sprite = libraryAsset.GetSprite("Equip", type);
    //}

    public void MiniMapPosition(PlayerMap map)
    {
        player.playerMap = map;

        switch (map)
        {
            case PlayerMap.Farm:
                playerImage.anchoredPosition = new Vector2(620, 200);
                break;
            case PlayerMap.FarmToTown:
                playerImage.anchoredPosition = new Vector2(267, 134);
                break;
            case PlayerMap.Town:
                playerImage.anchoredPosition = new Vector2(50, 134);
                break;
            case PlayerMap.TownToForest:
                playerImage.anchoredPosition = new Vector2(-176, 138);
                break;
            case PlayerMap.Forest:
                playerImage.anchoredPosition = new Vector2(-501, 151);
                break;
            case PlayerMap.FarmToForest:
                playerImage.anchoredPosition = new Vector2(29, 359);
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
            case PlayerMap.Home:
                playerImage.anchoredPosition = new Vector2(470, 130);
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
}
