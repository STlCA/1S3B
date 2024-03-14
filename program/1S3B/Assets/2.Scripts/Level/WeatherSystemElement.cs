using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystemElement : MonoBehaviour
{
    
    [ExecuteInEditMode]
    
        public WeatherSystem.WeatherType WeatherType;

        private void OnDestroy()
        {
            WeatherSystem.UnregisterElement(this);
        }
    
}
