using Constants;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : Manager
{
    private TileManager tileManager;
    private Player player;

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
    private Dictionary<Season, List<WeatherType>> seasonWeather = new();

    public Action<bool> IsRainAction;


    void Start()
    {
        tileManager = gameManager.TileManager;
        player = gameManager.Player;

        SeasonSetting();

        FindAllElements();
        ChangeWeather(startingWeather);
    }

    private void SeasonSetting()
    {
        seasonWeather[Season.Spring] = new List<WeatherType>() { WeatherType.Sun, WeatherType.Sun, WeatherType.Sun, WeatherType.Rain, WeatherType.Rain };
        seasonWeather[Season.Summer] = new List<WeatherType>() { WeatherType.Sun, WeatherType.Sun, WeatherType.Rain, WeatherType.Rain, WeatherType.Rain };
        seasonWeather[Season.Fall] = new List<WeatherType>() { WeatherType.Sun, WeatherType.Sun, WeatherType.Sun, WeatherType.Rain, WeatherType.Rain };
        seasonWeather[Season.Winter] = new List<WeatherType>() { WeatherType.Sun, WeatherType.Sun, WeatherType.Snow, WeatherType.Snow, WeatherType.Snow };
    }

    public static void UnregisterElement(WeatherSystemElement element)
    {
        GameManager.Instance?.WeatherSystem?.elements.Remove(element);

    }
    public void ChangeWeather(WeatherType newType)
    {
        currentWeatherType = newType;
        //현재날씨를 저장
        SwitchAllElementsToOutdoor(false);
        //날씨효과들에게 가서 이거 날씨 바뀌어야된다고 말해줌
    }

    void FindAllElements()
    {
        //시작할때 날씨객체들을 전부 찾아오겠다
        elements = new(GameObject.FindObjectsOfType<WeatherSystemElement>(true));
    }

/*    void SwitchAllElementsToCurrentWeather()
    {
        foreach (var element in elements)
        {
            element.gameObject.SetActive(element.WeatherType.HasFlag(currentWeatherType));
        }
    }*/

    public void SwitchAllElementsToOutdoor(bool isOutdoor)
    {
        foreach (var element in elements)
        {
            if (element.GetComponent<AudioSource>() != null)
                element.gameObject.SetActive(element.WeatherType.HasFlag(currentWeatherType));
            else
                element.gameObject.SetActive(isOutdoor && element.WeatherType.HasFlag(currentWeatherType));
        }
    }

    public void RandomChangeWeather(Season season)
    {
        List<WeatherType> temp = seasonWeather[season];

        int num = UnityEngine.Random.Range(0, temp.Count);

        WeatherType weatherType = temp[num];
        currentWeatherType = weatherType;

        IsRainAction?.Invoke(currentWeatherType == WeatherType.Rain);
        tileManager.IsRain(currentWeatherType == WeatherType.Rain);
        SwitchAllElementsToOutdoor(false);
    }
}