using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Constants;
using UnityEngine.Playables;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System;

[System.Serializable]
public struct SavePlayerData
{
    public string Name;
    public int Gold;
    public int Energy;
}

[System.Serializable]
public class PlayerSkill
{
    public string skillName;
    public int level;
    public float exp;
}

[System.Serializable]
public class PlayerEquimentLevel
{
    public PlayerEquipmentType equimentType;
    public int level;
    public float exp;
    public int count;
    public UpgradeEquipmentStep step;
}

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private UIManager uiManager;
    private WeatherSystem weatherSystem;

    public AnimationController animationController { get; private set; }
    public CharacterEventController characterEventController { get; private set; }

    public PlayerMovement playerMovement { get; private set; }

    public Item selectItem { get; set; }

    public string playerName { get; private set; }

    public Vector3 playerPosition { get; set; }
    public PlayerState playerState { get; private set; }

    public int PlayerGold
    {
        get => playerGold;
        set
        {
            playerGold = value;
            uiManager.UpdateGoldUI(PlayerGold);
        }
    }
    private int playerGold;

    public float playerSpeed { get; private set; }
    public int playerMaxEnergy { get; private set; } = 150;
    public int PlayerEnergy
    {
        get => playerEnergy;
        set
        {
            playerEnergy = value;
            uiManager.EnergyBarUpdate(PlayerEnergy);
        }
    }
    private int playerEnergy;

    private int useEnergyAmount = 2;


    public PlayerSkill[] playerSkills = new PlayerSkill[5];
    private string[] skillName;

    public PlayerEquimentLevel[] equipmentsLevel = new PlayerEquimentLevel[10];

    public Inventory Inventory { get { return inventory; } }
    private Inventory inventory;
    public QuickSlot QuickSlot { get { return quickSlot; } }
    private QuickSlot quickSlot;

    public Action<bool> IsDeathAction;

    [HideInInspector] public PlayerMap playerMap = PlayerMap.Farm;

    private void Awake()
    {
        animationController = GetComponent<AnimationController>();
        characterEventController = GetComponent<CharacterEventController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        uiManager = gameManager.UIManager;
        weatherSystem = gameManager.WeatherSystem;

        quickSlot = GetComponent<QuickSlot>();
        inventory = GetComponent<Inventory>();

        quickSlot.Init(gameManager);
        inventory.Init(gameManager);

        StateInit();

        characterEventController.OnClickEvent += PlusExp;
        characterEventController.OnClickEvent += PlusEquipmentExp;
        weatherSystem.IsRainAction += UseEnergyAmount;
    }

    private void StateInit()
    {
        playerState = PlayerState.IDLE;

        playerEnergy = playerMaxEnergy;
        PlayerGold = 1000;
        playerSpeed = 7f;
        transform.position = new Vector3(351f, 4.3f);

        skillName = new string[] { "농사", "벌목", "채광", "전투", "낚시" };

        for (int i = 0; i < equipmentsLevel.Length; i++)
        {
            equipmentsLevel[i] = new PlayerEquimentLevel();
            equipmentsLevel[i].equimentType = ((PlayerEquipmentType)i);
            equipmentsLevel[i].level = 1;
            equipmentsLevel[i].exp = 0;
            equipmentsLevel[i].count = 0;
            equipmentsLevel[i].step = UpgradeEquipmentStep.None;

        }

        for (int i = 0; i < playerSkills.Length; i++)
        {
            playerSkills[i] = new PlayerSkill();
            playerSkills[i].skillName = skillName[i];
            playerSkills[i].level = 1;
            playerSkills[i].exp = 0;
        }
    }

    private void UseEnergyAmount(bool isRain)
    {
        if (isRain == true)
            useEnergyAmount = 4;
        else
            useEnergyAmount = 2;
    }

    public void UseEnergy()
    {
        playerEnergy -= useEnergyAmount;
        uiManager.EnergyBarUpdate(playerEnergy);

        if (playerEnergy <= 0 && playerEnergy > -20)
        {
            playerSpeed = 2f;
            playerState = PlayerState.TIRED;
            uiManager.TiredIconOnOff(playerState == PlayerState.TIRED);
            animationController.AnimationSpeedChange(0.5f);
            GameManager.Instance.SoundManager.WalkSoundChange(false);
        }
        else if (playerEnergy <= -20 && playerState == PlayerState.TIRED)
        {
            PlayerGold -= GoldRange(10, 20);//골드세팅되고 sleep불러야함

            GameManager.Instance.PopUpController.SwitchPlayerInputAction(true);

            IsDeathAction?.Invoke(true);
        }
    }

    public void EnergyReset(bool status = false)
    {
        playerState = PlayerState.IDLE;
        playerSpeed = 7f;

        if (status == true)
        {
            playerEnergy = playerMaxEnergy / 2;
            uiManager.EnergyBarUpdate(playerEnergy);
            uiManager.TiredIconOnOff(false);
        }
        else
        {
            playerEnergy = playerMaxEnergy;
            uiManager.EnergyBarUpdate(playerMaxEnergy);
        }
    }

    private int GoldRange(int range1, int range2)
    {
        int value;

        int temp1 = PlayerGold / 100 * range1;
        int temp2 = PlayerGold / 100 * range2;

        value = UnityEngine.Random.Range(temp1, temp2 + 1);

        return value;
    }

    public void EnergyRecovery()
    {
        playerEnergy += 10;
        uiManager.EnergyBarUpdate(playerEnergy);
    }

    public void ChangePosition(Vector3 pos = default)
    {
        transform.position = playerPosition;
    }

    public void PlusEquipmentExp(PlayerEquipmentType equipmentType, Vector2 pos)
    {
        int temp = (int)equipmentType;

        equipmentsLevel[temp].exp += 1 / equipmentsLevel[temp].level;
        equipmentsLevel[temp].count++;

        if (equipmentsLevel[temp].exp >= 100)
        {
            equipmentsLevel[temp].exp = 0;
            equipmentsLevel[temp].level++;
            equipmentsLevel[temp].step = (UpgradeEquipmentStep)equipmentsLevel[temp].level - 1;
        }
    }

    public void PlusExp(PlayerEquipmentType equipmentType, Vector2 pos)
    {
        /*PlayerSkill currentSkill = playerSkill[skillType];
        currentSkill.exp += 1 / currentSkill.level;
        currentSkill.Count++;

        if(currentSkill.exp >= 100)
        {
            currentSkill.exp = 0;
            currentSkill.level++;
            currentSkill.step = (UpgradeEquipmentStep)currentSkill.level - 1;
        }    */

        PlayerSkillType skillType = new();

        switch (equipmentType)
        {
            case PlayerEquipmentType.PickUp:
            case PlayerEquipmentType.Hoe:
            case PlayerEquipmentType.Water:
                skillType = PlayerSkillType.Farming;
                break;
            case PlayerEquipmentType.Axe:
                skillType = PlayerSkillType.Felling;
                break;
            case PlayerEquipmentType.PickAxe:
                skillType = PlayerSkillType.Mining;
                break;
            case PlayerEquipmentType.Sword:
                skillType = PlayerSkillType.Battle;
                break;
            case PlayerEquipmentType.FishingRod:
                skillType = PlayerSkillType.Fishing;
                break;
        }


        int temp = (int)skillType;

        playerSkills[temp].exp += 1 / playerSkills[temp].level;

        if (playerSkills[temp].exp >= 100)
        {
            playerSkills[temp].exp = 0;
            playerSkills[temp].level++;
        }
    }

    public void PlayerStateChange(PlayerState state)
    {
        playerState = state;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crop"))
            playerSpeed = 5f;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Crop"))
            playerSpeed = 7f;
    }

    // 입금 
    //public void Deposit(int gold, int quantity)
    //{
    //    PlayerGold += gold * quantity;
    //}
    public void Deposit(int gold)
    {
        PlayerGold += gold;
    }

    // 출금
    //public void Withdraw(int gold, int quantity)
    //{
    //    PlayerGold -= gold * quantity;
    //}
    public bool Withdraw(int gold)
    {
        if (PlayerGold < gold)
        {
            return false;
        }

        PlayerGold -= gold;
        return true;
    }

    //==================================================Save

    public void PlayerNameSetting(string name)
    {
        playerName = name;
    }

    public void Save(ref SavePlayerData data)
    {
        data.Gold = PlayerGold;
        data.Energy = playerEnergy;
        data.Name = playerName;
    }

    public void Load(SavePlayerData data)
    {
        PlayerGold = data.Gold;
        PlayerEnergy = data.Energy;
        playerName = data.Name;
    }
}
