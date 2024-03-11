using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayCycleHandler : MonoBehaviour
{
    public Transform LightsRoot;

    [Header("Day Light")]
    public Light2D DayLight;
    public Gradient DayLightGradient;

    [Header("Night Light")]
    public Light2D NightLight;
    public Gradient NightLightGradient;

    [Header("Ambient Light")]
    public Light2D AmbientLight;
    public Gradient AmbientLightGradient;

    [Header("RimLights")]
    public Light2D SunRimLight;
    public Gradient SunRimLightGradient;
    public Light2D MoonRimLight;
    public Gradient MoonRimLightGradient;

    [Tooltip("The angle 0 = upward, going clockwise to 1 along the day")]
    public AnimationCurve ShadowAngle;
    [Tooltip("The scale of the normal shadow length (0 to 1) along the day")]
    public AnimationCurve ShadowLength;

    private List<LightInterpolator> m_LightBlenders = new();

    private void Awake()
    {
        GameManager.Instance.DayCycleHandler = this;
    }
}
