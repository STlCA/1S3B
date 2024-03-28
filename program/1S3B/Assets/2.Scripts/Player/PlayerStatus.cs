using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Constants;

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

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus instance;

    [HideInInspector] public Vector3 playerPosition;

    private PlayerState playerState;

    public Slider energyBar;
    private TMP_Text energyText;
    private int playerMaxEnergy = 150;
    [SerializeField] private int playerEnergy;//serial 나중에 지우기

    public GameObject tired;
    [HideInInspector] public bool isTired = false;

    [HideInInspector] public int playerGold = 1500;

    [HideInInspector] public float playerSpeed = 10f;

    [HideInInspector] public AnimationController animationController;
    private CharacterEventController characterEventController;

    private string[] skillName = new string[] { "농사", "벌목", "채광", "전투", "낚시" };
    public PlayerSkill[] playerSkills = new PlayerSkill[5];

    public PlayerEquimentLevel[] equipmentsLevel = new PlayerEquimentLevel[7];
    //public Dictionary<PlayerSkillType, PlayerSkill> playerSkill = new();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        energyText = energyBar.GetComponentInChildren<TMP_Text>();
        animationController = GetComponent<AnimationController>();
        characterEventController = GetComponent<CharacterEventController>();

        Init();

        playerSpeed = 7f;

        playerState = PlayerState.IDLE;

        characterEventController.OnClickEvent += PlusExp;
        characterEventController.OnClickEvent += PlusEquipmentExp;
    }

    private void Init()
    {
        energyBar.maxValue = playerMaxEnergy;
        energyBar.minValue = 0;
        EnergyUpdate(playerMaxEnergy);

        energyText.gameObject.SetActive(false);
        tired.SetActive(false);


        // skill init
        /*        for (int i = 0; i < skillName.Length; i++)
                {
                    PlayerSkillType skillType = (PlayerSkillType)i;

                    PlayerSkill tempPlayerSkill = new PlayerSkill();
                    tempPlayerSkill.skillName = skillName[i];
                    tempPlayerSkill.level = 1;
                    tempPlayerSkill.exp = 0;
                    tempPlayerSkill.count = 0;
                    tempPlayerSkill.step = UpgradeEquipmentStep.None;

                    playerSkill.Add(skillType, tempPlayerSkill);
                }*/

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
        EnergyUpdate(playerEnergy -= 2);

        if (playerEnergy <= 0 && playerEnergy > -20)
        {
            playerSpeed = 2f;
            isTired = true;
            tired.SetActive(true);
        }
        else if (playerEnergy <= -20 && playerState == PlayerState.IDLE)
        {
            playerState = PlayerState.TIRED;
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
            EnergyUpdate(playerMaxEnergy / 2);
            isTired = false;
            tired.SetActive(false);
        }
        else
        {
            EnergyUpdate(playerMaxEnergy);
        }
    }

    private void EnergyUpdate(int playerEnergy)
    {
        if (playerEnergy > playerMaxEnergy)
            playerEnergy = playerMaxEnergy;

        this.playerEnergy = playerEnergy;
        energyBar.value = playerEnergy;
        energyText.text = playerEnergy + "/" + energyBar.maxValue;
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
        EnergyUpdate(playerEnergy += 10);
    }

    public void ChangePosition()
    {
        gameObject.transform.position = playerPosition;
    }

    private void DeathSleep()
    {

        GameManager.Instance.SleepOfDay();
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
}
