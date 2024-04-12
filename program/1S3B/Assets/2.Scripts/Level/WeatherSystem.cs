using System;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : Manager
{
    private TileManager tileManager;

    [Flags]
    public enum WeatherType
    {
        Sun = 0x1,
        Rain = 0x2,
        Snow = 0x4
    }
    //플래그>복수선택가능(여기서는 16진수로 표현)

    public WeatherType startingWeather;
    //시작날씨(sun)

    private WeatherType currentWeatherType;
    //현재 날씨 타입
    private List<WeatherSystemElement> elements = new List<WeatherSystemElement>();

    void Start()
    {
        tileManager = gameManager.TileManager;

        FindAllElements();
        ChangeWeather(startingWeather);
    }
    public static void UnregisterElement(WeatherSystemElement element)
    {
            GameManager.Instance?.WeatherSystem?.elements.Remove(element);

    }
    public void ChangeWeather(WeatherType newType)
    {
        currentWeatherType = newType;
        //현재날씨를 저장
        SwitchAllElementsToCurrentWeather();
        //날씨효과들에게 가서 이거 날씨 바뀌어야된다고 말해줌
    }

    void FindAllElements()
    {
        //시작할때 날씨객체들을 전부 찾아오겠다
        elements = new(GameObject.FindObjectsOfType<WeatherSystemElement>(true));
    }

    void SwitchAllElementsToCurrentWeather()
    {
        foreach (var element in elements)
        {
            element.gameObject.SetActive(element.WeatherType.HasFlag(currentWeatherType));
        }
    }

    public void RandomChangeWeather()
    {
        int num = UnityEngine.Random.Range(0,3);
        switch (num)
        {
            case 0 :
                currentWeatherType = WeatherType.Sun;
                break;
            case 1:
                currentWeatherType = WeatherType.Rain;                
                break;
            case 2:
                currentWeatherType = WeatherType.Snow;
                break;
            
        }
        tileManager.IsRain(currentWeatherType == WeatherType.Rain);
        SwitchAllElementsToCurrentWeather();
    }
}