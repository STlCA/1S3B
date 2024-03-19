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




    private float m_CurrentTimeOfTheDay;
    //오늘의 현재 시간

    public DayCycleHandler DayCycleHandler { get; set; }
    public WeatherSystem WeatherSystem { get; set; }

    /* 
    Will return the ratio of time for the current day between 0 (00:00) and 1 (23:59).
    현재 날짜의 시간 비율을 0(00:00)에서 1(23:59) 사이에 반환합니다.
    현재 날짜의 시간 비율 => 오늘의 현재 시간 / 하루 지속 시간(초)
    */
    public float CurrentDayRatio => m_CurrentTimeOfTheDay / DayDurationInSeconds;


    [Header("Time settings")]
    [Min(1.0f)]
    //최솟값

    public float DayDurationInSeconds;
    public float StartingTime = 0.0f;
    //시작시간


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dataManager = GetComponentInChildren<DataManager>();

        DontDestroyOnLoad(gameObject);
    }


    private List<DayEventHandler> m_EventHandlers = new();

    public static void RegisterEventHandler(DayEventHandler handler)
    {
        foreach (var evt in handler.Events)
        {
            if (evt.IsInRange(GameManager.Instance.CurrentDayRatio))
            {
                evt.OnEvents.Invoke();
            }
            else
            {
                evt.OffEvent.Invoke();
            }
        }

        Instance.m_EventHandlers.Add(handler);
    }

    public static void RemoveEventHandler(DayEventHandler handler)
    {
        Instance?.m_EventHandlers.Remove(handler);
    }

    public static string GetTimeAsString(float ratio)
    {//문자열로 시간 가져오기
        var hour = GetHourFromRatio(ratio);
        //시간
        var minute = GetMinuteFromRatio(ratio);
        //분
        return $"{hour}:{minute:00}";
    }

    public static int GetHourFromRatio(float ratio)
    {//비율로 시간
        var time = ratio * 24.0f;
        var hour = Mathf.FloorToInt(time);

        return hour;
    }

    public static int GetMinuteFromRatio(float ratio)
    {//비율로 분
        var time = ratio * 24.0f;
        var minute = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 60.0f);

        return minute;
    }

    private void Update()
    {
        m_CurrentTimeOfTheDay += Time.deltaTime;

        while (m_CurrentTimeOfTheDay > DayDurationInSeconds)
            m_CurrentTimeOfTheDay -= DayDurationInSeconds;
        //하루가 120이면 120이 넘었을때 120을 빼고 1부터 시작

        if (DayCycleHandler != null)
            DayCycleHandler.Tick();
    }
}
