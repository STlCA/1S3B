using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Constants;
using UnityEngine.Playables;

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

    [HideInInspector] public AnimationController animationController;
    private CharacterEventController characterEventController;

    public Vector3 playerPosition { get; set; }
    public PlayerState playerState {  get; private set; }
    public int playerGold { get; private set; }
    public float playerSpeed { get; private set; }    
    public int playerMaxEnergy { get; private set; } = 150;
    private int playerEnergy;

    private string[] skillName;
    public PlayerSkill[] playerSkills = new PlayerSkill[5];

    public PlayerEquimentLevel[] equipmentsLevel = new PlayerEquimentLevel[7];

    private void Start()
    {
        gameManager = GameManager.Instance;
        uiManager = gameManager.UIManager;

        animationController = GetComponent<AnimationController>();
        characterEventController = GetComponent<CharacterEventController>();

        Init();        

        playerState = PlayerState.IDLE;

        characterEventController.OnClickEvent += PlusExp;
        characterEventController.OnClickEvent += PlusEquipmentExp;
    }

    private void Init()
    {
        playerEnergy = playerMaxEnergy;
        playerGold = 1500;
        playerSpeed = 7f;

        skillName = new string[] { "농사", "벌목", "채광", "전투", "낚시" };

        for (int i = 0; i < equipmentsLevel.Length; i++)
        {
            equipmentsLevel[i] = new PlayerEquimentLevel();
            equipmentsLevel[i].equimentType = ((PlayerEquipmentType)i);
            equipmentsLevel[i].level = 1;
            equipmentsLevel[i].exp = 0;
            equipmentsLevel[i].count = 0; ;
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

    public void UseEnergy()
    {
        playerEnergy -= 2;
        uiManager.EnergyUpdate(playerEnergy);

        if (playerEnergy <= 0 && playerEnergy > -20)
        {
            playerSpeed = 2f;
            playerState = PlayerState.TIRED;
            uiManager.TiredIconOnOff(playerState== PlayerState.TIRED);
        }
        else if (playerEnergy <= -20 && playerState == PlayerState.TIRED)
        {
            playerState = PlayerState.BLACKOUT;
            animationController.DeathAnimation(true);
            Invoke("DeathSleep", 1f);

            //DeathSleep();

            playerGold -= GoldRange(10, 20);
        }
    }

    public void EnergyReset(bool status = false)
    {
        if (status == true)
        {
            playerState = PlayerState.IDLE;
            playerSpeed = 10f;
            playerEnergy = playerMaxEnergy / 2;
            uiManager.EnergyUpdate(playerEnergy);
            playerState = PlayerState.IDLE;
            uiManager.TiredIconOnOff(false);
        }
        else
            uiManager.EnergyUpdate(playerMaxEnergy);
    }

    private int GoldRange(int range1, int range2)
    {
        int value;

        int temp1 = playerGold / 100 * range1;
        int temp2 = playerGold / 100 * range2;

        value = Random.Range(temp1, temp2 + 1);

        return value;
    }

    public void EnergyRecovery()
    {
        playerEnergy += 10;
        uiManager.EnergyUpdate(playerEnergy);
    }

    public void ChangePosition()
    {
        transform.position = playerPosition;
    }

    private void DeathSleep()
    {
        gameManager.SleepOfDay();
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
        currentSkill.count++;

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
}