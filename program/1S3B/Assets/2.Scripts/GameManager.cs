using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TargetSetting targetSetting { get; set; }
    public TileManager tileManager { get; set; }//물어보기
    public SceneChangeManager sceneChangeManager { get; set; }
    public UIManager uIManager { get; set; }

    public DataManager dataManager;
    public TalkManager talkManager;

    public DayCycleHandler dayCycleHandler { get; set; }
    public WeatherSystem weatherSystem { get; set; }

    [Header("Time")]
    public TMP_Text TimeText;

    [Header("Talk")]
    public GameObject talkPanel;
    public TMP_Text talkText;
    public TMP_Text npcNameText;
    public Image portraitImg;
    public GameObject scanObject;
    private bool isTalk;
    private int talkIndex;

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

    public void NpcTalkAction(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjectData objectData = scanObj.GetComponent<ObjectData>();
        NpcTalk(objectData.id, objectData.isNpc);

        talkPanel.SetActive(isTalk);
    }

    private void NpcTalk(int id, bool isNpc)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        string npcName = talkManager.GetNpcName(id);

        if(talkData == null)
        {
            isTalk = false;
            talkIndex = 0;
            return;
        }

        if(isNpc)
        {
            talkText.text = talkData.Split(':')[0];
            portraitImg.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
            portraitImg.color = new Color(1, 1, 1, 1);
            npcNameText.text = npcName;
        }

        isTalk = true;
        talkIndex++;
    }

}
