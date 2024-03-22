using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherTest : MonoBehaviour
{

    public WeatherSystem weatherSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            weatherSystem.ChangeWeather(WeatherSystem.WeatherType.Rain);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            weatherSystem.ChangeWeather(WeatherSystem.WeatherType.Sun);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            weatherSystem.ChangeWeather(WeatherSystem.WeatherType.Snow);
        }
    }
}
