using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager I;

    public Light2D globalLight; // Light2D 컴포넌트 참조
    public Color dayColor = Color.white; // 낮 색
    public Color nightColor = new Color(30 / 255f, 130 / 255f, 255 / 255f); // 밤 색
    public float transitionDuration = 10.0f; // 낮,밤 전환 시간

    private float realTime = 1;//실제시간몇초당 10분
    private float time;
    private int gameTime = 360;

    private void Awake()
    {
        I = this;
    }

    private void Update()
    {
        UpdateLightingTransition();
    }

    public void TimeCheck()
    {
        time += Time.deltaTime;

        if (time >= realTime)
        {
            gameTime += 10;
            time = 0;

        }
    }

    public bool IsMorning() // 오전 7시부터 아침, 아침 되면 몬스터 리스폰
    {
        return gameTime >= 420 && gameTime < 430;
    }

    public bool IsNight() // 오후 6시부터 다음날 오전 6시까지 밤으로 판단
    {
        return gameTime >= 1080 || gameTime < 360; // 1080은 오후 6시, 360은 오전 6시를 의미
    }

    private void UpdateLightingTransition()
    {
        float currentHour = gameTime / 60.0f; // 현재 시간을 시간 단위로 변환
        float lerpFactor;

        // 오전 6시부터 낮 색상으로 전환 시작
        if (currentHour >= 6f && currentHour < 7f) // 오전 6시부터 1시간 동안
        {
            lerpFactor = (currentHour - 6f) / 1f; // 오전 6시부터 1시간 동안 보간 계산
            globalLight.color = Color.Lerp(nightColor, dayColor, lerpFactor);
        }
        else if (currentHour >= 17f && currentHour < 18f) // 오후 5시부터 1시간 동안 밤으로 전환 시작
        {
            lerpFactor = (currentHour - 17f) / 1f; // 오후 5시부터 1시간 동안 보간
            globalLight.color = Color.Lerp(dayColor, nightColor, lerpFactor);
        }
        else if (currentHour >= 7f && currentHour < 17f) // 오전 7시부터 오후 5시까지는 낮 색상 유지
        {
            globalLight.color = dayColor;
        }
        else // 그 외 시간에는 밤 색상 유지
        {
            globalLight.color = nightColor;
        }
    }
}
