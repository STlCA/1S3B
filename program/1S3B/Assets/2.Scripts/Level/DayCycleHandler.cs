using System;
using System.Collections.Generic;
using Constants;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

[System.Serializable]
public struct SaveDayData
{
    public int Day;
    public int Season;
}

public class DayCycleHandler : Manager
{
    private float currentTime;
    //오늘의 현재 시간(누적되는시간)
    private float priviousTime;
    //이전시간

    /* 
    Will return the ratio of time for the current day between 0 (00:00) and 1 (23:59).
    현재 날짜의 시간 비율을 0(00:00)에서 1(23:59) 사이에 반환합니다.
    현재 날짜의 시간 비율 => 오늘의 현재 시간 / 하루 지속 시간(초)
    */
    public float currentDayRatio => currentTime / dayDurationInSeconds;
    public int currentDay { private set; get; }
    public Season currentSeason { private set; get; } = Season.Spring;

    List<string> week = new List<string> { "월요일", "화요일", "수요일", "목요일", "금요일", "토요일", "일요일", };

    public Action<Season> changeSeasonAction;

    [Header("Time settings")]
    [Min(1.0f)]
    //최솟값(인스펙터에서 조절할때)
    public float dayDurationInSeconds;
    public float startingTime = 0.0f;
    //시작시간
    public float endTime = 0.0f;
    //끝나는시간

    [Header("Day Light")]
    public Light2D dayLight;
    public Gradient dayLightGradient;

    [Header("Night Light")]
    public Light2D nightLight;
    public Gradient nightLightGradient;

    [Header("Ambient Light")]
    public Light2D ambientLight;
    public Gradient ambientLightGradient;

    public override void Init(GameManager gm)
    {
        base.Init(gm);

        if (dayDurationInSeconds <= 0.0f)
            dayDurationInSeconds = 5.0f;

        ResetDayTime();
    }


    public void Tick()
    {
        UpdateTime();
        DayEndTime();
        UpdateLight(currentDayRatio);
    }

    private void UpdateTime()
    {
        priviousTime = currentTime;
        currentTime += Time.deltaTime;

        while (currentTime > dayDurationInSeconds)
            currentTime -= dayDurationInSeconds;
        //하루가 120이면 120이 넘었을때 120을 빼고 1부터 시작
    }

    private void DayEndTime()
    {
        //이전시간과 현재시간의 중간에 끝나는시간이있어야함
        if (priviousTime < endTime && endTime <= currentTime)
        {
            gameManager.DayOverTime(true);
        }
    }

    public void ResetDayTime()
    {
        currentTime = startingTime;
        //하루시작시간 초기화
        priviousTime = currentTime;
    }

    public void UpdateLight(float ratio)
    {
        dayLight.color = dayLightGradient.Evaluate(ratio);
        nightLight.color = nightLightGradient.Evaluate(ratio);

        ambientLight.color = ambientLightGradient.Evaluate(ratio);

    }

    /*
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
    */
    public string[] GetTimeAsString()
    {
        return GetTimeAsString(currentDayRatio);
    }

    public string[] GetTimeAsString(float ratio)
    {//문자열로 시간 가져오기
        var hour = GetHourFromRatio(ratio);
        //시간
        var minute = GetMinuteFromRatio(ratio);
        //분

        int adjustedMinute = (int)(minute / 10) * 10;
        //텍스트에서는 분이 10분 단위로 보이게

        string AmPm;            

        if (hour < 13)
            AmPm = "오전";

        else
        {
            hour = hour - 12;
            AmPm = "오후";
        }

        string[] returnArray = new string[] {AmPm,$"{hour:00} : {adjustedMinute:00}" };

        //return $"{hour:00} : {adjustedMinute:00}{AmPm}";
        return returnArray;
    }

    public int GetHourFromRatio(float ratio)
    {//비율로 시간
        var time = ratio * 24.0f;
        var hour = Mathf.FloorToInt(time);

        return hour;
    }

    public int GetMinuteFromRatio(float ratio)
    {//비율로 분
        var time = ratio * 24.0f;
        var minute = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 60.0f);

        return minute;
    }

    public void ChangeDate()
    {
        currentDay++;

        if (currentDay % 28 == 0)
        {
            if (currentSeason == Season.Winter)
            {
                currentSeason = Season.Spring;
            }
            else
                currentSeason++;

            changeSeasonAction?.Invoke(currentSeason);
        }
    }

    public string[] GetDayAsString()
    {//문자열로 날짜표시하기
        int dayofWeek = currentDay % 7;
        int day = currentDay % 28 + 1;

        string[] returnArray = new string[] { week[dayofWeek], day.ToString() };

        return returnArray;
        //return $"{week[dayofWeek]}  {day}";
    }

    public void DateTest()
    {
        currentDay += 26;
        foreach(var (cell, data) in GameManager.Instance.TileManager.cropData)
        {
            data.deathTimer = data.plantCrop.DeathTimer - currentDay % 28;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DayCycleHandler))]
    class DayCycleEditor : Editor
    {
        private DayCycleHandler m_Target;

        public override VisualElement CreateInspectorGUI()
        {
            m_Target = target as DayCycleHandler;

            var root = new VisualElement();

            UnityEditor.UIElements.InspectorElement.FillDefaultInspector(root, serializedObject, this);

            var slider = new Slider(0.0f, 1.0f);
            slider.label = "Test time 0:00";
            slider.RegisterValueChangedCallback(evt =>
            {
                m_Target.UpdateLight(evt.newValue);

                slider.label = $"Test Time {m_Target.GetTimeAsString(evt.newValue)} ({evt.newValue:F2})";
                SceneView.RepaintAll();
            });

            //registering click event, it's very catch all but not way to do a change check for control change
            root.RegisterCallback<ClickEvent>(evt =>
            {
                m_Target.UpdateLight(slider.value);
                SceneView.RepaintAll();
            });

            root.Add(slider);

            return root;
        }

    }
#endif

    //============================================Save

    public void Save(ref SaveDayData data)
    {
        data.Day = currentDay;
        data.Season = (int)currentSeason;
    }

    public void Load(SaveDayData data)
    {
        currentDay = data.Day;
        currentSeason = (Season)data.Season;
        GameManager.Instance.UIManager.ChangeSeasonImage(currentSeason);
    }
}
