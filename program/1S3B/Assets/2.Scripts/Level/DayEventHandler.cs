using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayEventHandler : MonoBehaviour
{
    public class DayEvent
    {
        public float StartTime = 0.0f;
        public float EndTime = 1.0f;

        public UnityEvent OnEvents;
        public UnityEvent OffEvent;

        public bool IsInRange(float t)
        {
            return t >= StartTime && t <= EndTime;
            //스타트시간과 끝나는시간의 조건을 만족할때 범위내의 시간을 리턴
        }
    }

    public DayEvent[] Events;

    private void Start()
    {
        GameManager.RegisterEventHandler(this);
    }

    private void OnDisable()
    {
        GameManager.RemoveEventHandler(this);
    }
}

