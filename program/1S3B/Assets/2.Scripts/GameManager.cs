using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TargetSetting targetSetting { get; set; }
    public TileManager tileManager { get; set; }//물어보기
    public SceneChangeManager sceneChangeManager { get; set; }
    public UIManager uIManager { get; set; }

    public DataManager dataManager;


    public DayCycleHandler dayCycleHandler { get; set; }
    public WeatherSystem weatherSystem { get; set; }

    [Header("Time")]
    public TMP_Text TimeText;

    [HideInInspector] public PlayerMap playerMap = PlayerMap.Farm;//임시위치

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dataManager = GetComponentInChildren<DataManager>();

        dayCycleHandler = GetComponentInChildren<DayCycleHandler>();
        dayCycleHandler.Init(this);
        weatherSystem = GetComponentInChildren<WeatherSystem>();

        DontDestroyOnLoad(gameObject);

    }


    private void Update()
    {
        
        if (dayCycleHandler != null)
            dayCycleHandler.Tick();

        if(TimeText != null)
       
            TimeText.text = dayCycleHandler.GetTimeAsString();
            //시간텍스트 바꾸기

    }

    public void DayOverTime()
    {
        //EndTime넘어섯을때
        SleepOfDay();
    }

    public void SleepOfDay()
    {
        StartCoroutine(sceneChangeManager.SleepFadeInOut());
        Time.timeScale = 0.0f;
        tileManager.Sleep();
        bool status = PlayerStatus.instance.isTired;

        if (status == true)
            PlayerStatus.instance.EnergyReset(status);
        else
            PlayerStatus.instance.EnergyReset();

        dayCycleHandler.ResetDayTime();
        weatherSystem.RandomChangeWeather();
    }
}
