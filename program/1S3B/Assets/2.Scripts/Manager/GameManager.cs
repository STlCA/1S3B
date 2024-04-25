using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D.Animation;

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
    public PopUpController PopUpController { get { return popUpController; } }
    private PopUpController popUpController;
    public SoundSystemManager SoundManager { get { return soundManager; } }
    private SoundSystemManager soundManager;

    //========================Player
    public Player Player { get { return player; } }
    private Player player;
    public TargetSetting TargetSetting { get { return targetSetting; } }
    private TargetSetting targetSetting;
    public AnimationController AnimationController { get { return animationController; } }
    private AnimationController animationController;

    //========================Inspector

    [Header("Time")]
    public TMP_Text TimeText1;
    public TMP_Text TimeText2;

    [Header("Day")]
    public TMP_Text DayText1;
    public TMP_Text DayText2;

    [Header("Talk")]
    public GameObject talkPanel;
    public TMP_Text talkText;
    public TMP_Text npcNameText;

    //============================================
    public T GetManager<T>() where T : Manager
    {
        T t = GetComponentInChildren<T>();
        //t.Init(this);
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
        #region 싱글톤
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        #endregion

        //Find
        tileManager = GetManager<TileManager>();
        dataManager = GetManager<DataManager>();
        dayCycleHandler = GetManager<DayCycleHandler>();
        weatherSystem = GetManager<WeatherSystem>();
        sceneChangeManager = GetManager<SceneChangeManager>();
        natureObjectController = GetManager<NatureObjectController>();
        uIManager = GetManager<UIManager>();
        popUpController = GetManager<PopUpController>();
        soundManager = GetManager<SoundSystemManager>();

        player = GetFind<Player>();
        targetSetting = GetFind<TargetSetting>();
        animationController = player.GetComponent<AnimationController>();//물어보기

        //Init
        dataManager.Init(this);
        dayCycleHandler.Init(this);
        weatherSystem.Init(this);
        sceneChangeManager.Init(this);
        natureObjectController.Init(this);
        uIManager.Init(this);
        popUpController.Init(this);
        soundManager.Init(this);
        player.Init(this);
        tileManager.Init(this);
    }

    private void Start()
    {
        DayText1.text = DayCycleHandler.GetDayAsString()[0];
        DayText2.text = DayCycleHandler.GetDayAsString()[1];
    }

    private void Update()
    {
        if (DayCycleHandler != null)
            DayCycleHandler.Tick();

        if (TimeText1 != null && TimeText2 != null)
        {
            string[] temp = DayCycleHandler.GetTimeAsString();
            TimeText1.text = temp[0];
            TimeText2.text = temp[1];
        }

        //시간텍스트 바꾸기
    }

    public void DayOverTime()
    {
        //EndTime넘어섯을때
        //체력다써서 기절했을때
        player.playerPosition = new Vector3(351f, 4.3f);
        StartCoroutine(SleepOfDay(true));
    }

    public void SleepDayOver()
    {
        player.playerPosition = new Vector3(351f, 4.3f);
        StartCoroutine(SleepOfDay(false));
    }

    public IEnumerator SleepOfDay(bool isDeath)
    {
        popUpController.SwitchPlayerInputAction(true);

        soundManager.BGMSource.Stop();

        bool isTired = player.playerState == PlayerState.TIRED;
        player.PlayerStateChange(PlayerState.SLEEP);

        yield return StartCoroutine(SceneChangeManager.SleepFadeIn());

        player.EnergyReset(isTired);

        player.ChangePosition();

        TileManager.Sleep();

        dayCycleHandler.ChangeDate();
        DayText1.text = DayCycleHandler.GetDayAsString()[0];
        DayText2.text = DayCycleHandler.GetDayAsString()[1];

        DayCycleHandler.ResetDayTime();
        WeatherSystem.RandomChangeWeather(dayCycleHandler.currentSeason);//TileManager Sleep보다 아래여야함

        natureObjectController.SpawnNature();
        natureObjectController.PointSpawnTree(50);
        natureObjectController.PointSpawnStone(50);

        natureObjectController.RangeSpawnTree(1, SpawnPlace.UpForest);
        natureObjectController.RangeSpawnTree(1, SpawnPlace.DownForest);
        natureObjectController.RangeSpawnStone(2, SpawnPlace.Quarry);

        animationController.AnimationSpeedChange(1);

        SaveSystem.Save(player.playerName);

        soundManager.BGMSource.Play();
        yield return StartCoroutine(SceneChangeManager.SleepFadeOut());

        popUpController.SwitchPlayerInputAction(false);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
