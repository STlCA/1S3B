using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicOptionSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown; // �ػ� ���� ��Ӵٿ�
    [SerializeField] private TMP_Dropdown setFrameDropdown; // ������ ���� ��Ӵٿ�

    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Toggle showFrameToggle;

    [SerializeField] private TextMeshProUGUI showFPS_Txt; // FPS �� ǥ�� �ؽ�Ʈ

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

    private float fpsUpdateInterval = 0.5f; // 0.5�� ���� fps �ؽ�Ʈ ����
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
        // Time.unscaledDeltaTime -> ���� �ð� ����, Ÿ�� �����Ͽ� ���� �ȹ���

        float fps = 1f / Time.unscaledDeltaTime; // ���� �������� �ð����� �ʴ� ������ ��(FPS) ���ϱ�
        showFPS_Txt.text = $"FPS: {Mathf.RoundToInt(fps)}"; // �ݿø�

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
        ApplyGraphicsSettings(); // �ʱ� ���� ����
    }

    private void SetupToggles()
    {
        fullScreenToggle.isOn = true; // ��ü ȭ�� �⺻ On
        vSyncToggle.isOn = false; // ���� ����ȭ �⺻ Off
    }

    private void SetupFrameRateOptions()
    {
        List<string> frameOptions = new List<string> { "30", "60", "144", "165", "240" };
        setFrameDropdown.ClearOptions();
        setFrameDropdown.AddOptions(frameOptions);
        setFrameDropdown.value = 2;  // �⺻ ������ 144 �ε���
        setFrameDropdown.RefreshShownValue();
        OnFrameRateChange();  // �ʱ� ������ ����
    }

    public void OnFrameRateChange()
    {
        string selectedOption = setFrameDropdown.options[setFrameDropdown.value].text;
        Application.targetFrameRate = int.Parse(selectedOption);
    }


    public void ApplyGraphicsSettings()
    {
        Resolution selectedResolution = allowedResolutions[resolutionDropdown.value]; // �ػ� ����
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullScreenToggle.isOn);

        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0; // ���� ���� On/OFF

        OnFrameRateChange();  // ������ ���� �缳��
    }
}

