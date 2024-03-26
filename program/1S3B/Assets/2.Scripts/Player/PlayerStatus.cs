using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Constants;


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

        Init();

        playerSpeed = 7f;

        playerState = PlayerState.IDLE;

        animationController = GetComponent<AnimationController>();
    }

    private void Init()
    {
        energyBar.maxValue = playerMaxEnergy;
        energyBar.minValue = 0;        
        EnergyUpdate(playerMaxEnergy);

        energyText.gameObject.SetActive(false);
        tired.SetActive(false);
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
        else if(playerEnergy <= -20 && playerState == PlayerState.IDLE)
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
        if(status == true)
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

}
