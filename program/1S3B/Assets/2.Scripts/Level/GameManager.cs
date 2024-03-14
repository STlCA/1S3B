using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private float m_CurrentTimeOfTheDay;

    public DayCycleHandler DayCycleHandler { get; set; }
    public WeatherSystem WeatherSystem { get; set; }

    // Will return the ratio of time for the current day between 0 (00:00) and 1 (23:59).
    public float CurrentDayRatio => m_CurrentTimeOfTheDay / DayDurationInSeconds;


    [Header("Time settings")]
    [Min(1.0f)]
    //최솟값
    public float DayDurationInSeconds;
    public float StartingTime = 0.0f;
    //시간


    private void Awake()
    {
        Instance = this;
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
    {
        var hour = GetHourFromRatio(ratio);
        var minute = GetMinuteFromRatio(ratio);

        return $"{hour}:{minute:00}";
    }

    public static int GetHourFromRatio(float ratio)
    {
        var time = ratio * 24.0f;
        var hour = Mathf.FloorToInt(time);

        return hour;
    }

    public static int GetMinuteFromRatio(float ratio)
    {
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
