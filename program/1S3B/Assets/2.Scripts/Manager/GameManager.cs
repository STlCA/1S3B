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

    //======================Manager
    public TileManager TileManager { get { return tileManager; } }
    private TileManager tileManager;
    public NatureObjectController NatureObjectController { get { return natureObjectController; } }
    private NatureObjectController natureObjectController;
    public SceneChangeManager SceneChangeManager { get { return sceneChangeManager; } }
    private SceneChangeManager sceneChangeManager;
    public UIManager UIManager { get { return uIManager; } }
    private UIManager uIManager;
    public DataManager DataManager { get { return dataManager; } }
    private DataManager dataManager;
    public DayCycleHandler DayCycleHandler { get { return dayCycleHandler; } }
    private DayCycleHandler dayCycleHandler;
    public WeatherSystem WeatherSystem { get { return weatherSystem; } }
    private WeatherSystem weatherSystem;

    //========================Player
    public Player Player { get { return player; } }
    private Player player;
    public TargetSetting TargetSetting { get { return targetSetting; } }
    private TargetSetting targetSetting;
    public AnimationController AnimationController { get { return animationController; } }
    private AnimationController animationController;

    //========================Inspector
    [HideInInspector] public PlayerMap playerMap = PlayerMap.Farm;//임시위치

    [Header("Time")]
    public TMP_Text TimeText;

    [Header("Day")]
    public TMP_Text DayText;

    [Header("Talk")]
    public GameObject talkPanel;
    public TMP_Text talkText;
    public TMP_Text npcNameText;

    //============================================
    public T GetManager<T>() where T : Manager
    {
        T t = GetComponentInChildren<T>();
        t.Init(this);
        return t;
    }

    public T GetFind<T>() where T : MonoBehaviour
    {
        T t = FindObjectOfType<T>();
        return t;
    }

    //==============================================

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        tileManager = GetManager<TileManager>();
        dataManager = GetManager<DataManager>();
        dayCycleHandler = GetManager<DayCycleHandler>();
        weatherSystem = GetManager<WeatherSystem>();
        sceneChangeManager = GetManager<SceneChangeManager>();
        natureObjectController = GetManager<NatureObjectController>();
        uIManager = GetManager<UIManager>();

        player = GetFind<Player>();
        targetSetting = GetFind<TargetSetting>();
        animationController = player.GetComponent<AnimationController>();//물어보기

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        DayText.text = DayCycleHandler.GetDayAsString();
        talkPanel.SetActive(false);
    }

    private void Update()
    {
        if (DayCycleHandler != null)
            DayCycleHandler.Tick();

        if (TimeText != null)
        
            TimeText.text = DayCycleHandler.GetTimeAsString();
            //시간텍스트 바꾸기
        

    }

    public void DayOverTime()
    {
        //EndTime넘어섯을때
        SleepOfDay();
    }

    public void SleepOfDay()
    {
        player.PlayerStateChange(PlayerState.SLEEP);

        StartCoroutine(SceneChangeManager.SleepFadeInOut());
        TileManager.Sleep();

        player.EnergyReset(player.playerState == PlayerState.TIRED);

        DayCycleHandler.ResetDayTime();
        WeatherSystem.RandomChangeWeather();
        //natureObjectController.SpawnNature();
        //natureObjectController.PointSpawnTree();

        natureObjectController.RangeSpawnTree(10,SpawnType.Farm);
        natureObjectController.RangeSpawnStone(10,SpawnType.Farm);

        natureObjectController.RangeSpawnTree(10, SpawnType.UpForest);
        natureObjectController.RangeSpawnTree(10, SpawnType.DownForest);
        natureObjectController.RangeSpawnStone(10, SpawnType.Quarry);

        dayCycleHandler.ChangeDate();
        DayText.text = DayCycleHandler.GetDayAsString();
    }

}
