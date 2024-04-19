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
        StartCoroutine(SleepOfDay());
    }

    public IEnumerator SleepOfDay()
    {
        bool isTired = player.playerState == PlayerState.TIRED;
        player.PlayerStateChange(PlayerState.SLEEP);

        yield return StartCoroutine(SceneChangeManager.SleepFadeIn());

        player.EnergyReset(isTired);

        TileManager.Sleep();

        DayCycleHandler.ResetDayTime();
        WeatherSystem.RandomChangeWeather();//TileManager Sleep보다 아래여야함

        natureObjectController.SpawnNature();
        natureObjectController.PointSpawnTree(50);
        natureObjectController.PointSpawnStone(50);

        natureObjectController.RangeSpawnTree(1, SpawnPlace.UpForest);
        natureObjectController.RangeSpawnTree(1, SpawnPlace.DownForest);
        natureObjectController.RangeSpawnStone(2, SpawnPlace.Quarry);

        dayCycleHandler.ChangeDate();
        DayText.text = DayCycleHandler.GetDayAsString();

        yield return StartCoroutine(SceneChangeManager.SleepFadeOut());
    }

}
