using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Flags]
    public enum WeatherType
    {
        Sun = 0x1,
        Rain = 0x2,
        Thunder = 0x4
    }
    //플래그>복수선택가능(여기서는 16진수로 표현)

    public WeatherType StartingWeather;
    //시작날씨(sun)

    private WeatherType m_CurrentWeatherType;
    private List<WeatherSystemElement> m_Elements = new List<WeatherSystemElement>();

    private void Awake()
    {
       
    }

    void Start()
    {
        GameManager.Instance.WeatherSystem = this;
        FindAllElements();
        ChangeWeather(StartingWeather);
    }
    public static void UnregisterElement(WeatherSystemElement element)
    {
#if UNITY_EDITOR
        //in the editor when not running, we find the instance manually. Less efficient but not a problem at edit time
        //allow to be able to previz shadow in editor 
        if (!Application.isPlaying)
        {
            var instance = GameObject.FindObjectOfType<WeatherSystem>();
            if (instance != null)
            {
                instance.m_Elements.Remove(element);
            }
        }
        else
        {
#endif
            GameManager.Instance?.WeatherSystem?.m_Elements.Remove(element);
#if UNITY_EDITOR
        }
#endif
    }
    public void ChangeWeather(WeatherType newType)
    {
        m_CurrentWeatherType = newType;
        //현제날씨를 저장
        SwitchAllElementsToCurrentWeather();
        //날씨효과들 한태가서 이거 날씨 바뀌어야된다고 말해줌
    }

    void FindAllElements()
    {
        //날씨객체들을 전부 찾아오겠다
        m_Elements = new(GameObject.FindObjectsOfType<WeatherSystemElement>(true));
    }

    void SwitchAllElementsToCurrentWeather()
    {
        foreach (var element in m_Elements)
        {
            element.gameObject.SetActive(element.WeatherType.HasFlag(m_CurrentWeatherType));
        }
    }
}