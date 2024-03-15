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
    //현재 날씨 타입
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
        // 실행 중이 아닌 편집기에서는 인스턴스를 수동으로 찾습니다. 효율성은 떨어지지만 편집 시에는 문제가 되지 않습니다.
        // 편집기에서 그림자 미리보기를 가능하게 합니다
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
        //현재날씨를 저장
        SwitchAllElementsToCurrentWeather();
        //날씨효과들에게 가서 이거 날씨 바뀌어야된다고 말해줌
    }

    void FindAllElements()
    {
        //시작할때 날씨객체들을 전부 찾아오겠다
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