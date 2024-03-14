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

    public DataManager dataManager;


    private float m_CurrentTimeOfTheDay;

    public DayCycleHandler DayCycleHandler { get; set; }

    // Will return the ratio of time for the current day between 0 (00:00) and 1 (23:59).
    public float CurrentDayRatio => m_CurrentTimeOfTheDay / DayDurationInSeconds;


    [Header("Time settings")]
    [Min(1.0f)]
    public float DayDurationInSeconds;
    public float StartingTime = 0.0f;


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

}
