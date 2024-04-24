using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicOptionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // 해상도 설정 드롭다운
    [SerializeField] private TMP_Dropdown setFrameDropdown; // 프레임 설정 드롭다운

    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Toggle showFrameToggle;

    [SerializeField] private TextMeshProUGUI showFPS_Txt; // FPS 값 표시 텍스트

    private List<Resolution> allowedResolutions = new List<Resolution>()
    {
        new Resolution { width = 640, height = 360 },
        new Resolution { width = 800, height = 450 },
        new Resolution { width = 864, height = 486 },
        new Resolution { width = 960, height = 540 },
        new Resolution { width = 1024, height = 576 },
        new Resolution { width = 1152, height = 648 },
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1366, height = 768 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2048, height = 1152 },
        new Resolution { width = 2560, height = 1440 }
    };

    private float fpsUpdateInterval = 0.5f; // 0.5초 마다 fps 텍스트 갱신
    private float fpsUpdateTime = 0;

    void Start()
    {
        SetupResolutions();
        SetupToggles();
        SetupFrameRateOptions();
    }

    void Update()
    {
        if (showFrameToggle.isOn)
        {
            if (Time.unscaledTime > fpsUpdateTime)
            {
                ShowFPS();
                fpsUpdateTime = Time.unscaledTime + fpsUpdateInterval;
            }
        }
        else
        {
            showFPS_Txt.gameObject.SetActive(false);
        }
    }

    private void ShowFPS()
    {
        // Time.unscaledDeltaTime -> 실제 시간 기준, 타임 스케일에 영향 안받음

        float fps = 1f / Time.unscaledDeltaTime; // 이전 프레임의 시간으로 초당 프레임 수(FPS) 구하기
        showFPS_Txt.text = $"FPS: {Mathf.RoundToInt(fps)}"; // 반올림

        showFPS_Txt.gameObject.SetActive(true);
    }


    private void SetupResolutions()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < allowedResolutions.Count; i++)
        {
            Resolution res = allowedResolutions[i];
            string option = $"{res.width} x {res.height}";
            options.Add(option);
            if (res.width == 1920 && res.height == 1080)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        ApplyGraphicsSettings(); // 초기 설정 적용
    }

    private void SetupToggles()
    {
        fullScreenToggle.isOn = true; // 전체 화면 기본 On
        vSyncToggle.isOn = false; // 수직 동기화 기본 Off
    }

    private void SetupFrameRateOptions()
    {
        List<string> frameOptions = new List<string> { "30", "60", "144", "165", "240" };
        setFrameDropdown.ClearOptions();
        setFrameDropdown.AddOptions(frameOptions);
        setFrameDropdown.value = 2;  // 기본 프레임 144 인덱스
        setFrameDropdown.RefreshShownValue();
        OnFrameRateChange();  // 초기 프레임 설정
    }

    public void OnFrameRateChange()
    {
        string selectedOption = setFrameDropdown.options[setFrameDropdown.value].text;
        Application.targetFrameRate = int.Parse(selectedOption);
    }


    public void ApplyGraphicsSettings()
    {
        Resolution selectedResolution = allowedResolutions[resolutionDropdown.value]; // 해상도 설정
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullScreenToggle.isOn);

        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0; // 수직 동기 On/OFF

        OnFrameRateChange();  // 프레임 제한 재설정
    }
}

