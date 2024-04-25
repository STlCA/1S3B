using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [Header("UIImage")]
    public Image fadeImage;
    public GameObject loadingSliderObj;
    public Slider loadingSlider;

    [Header("SlotUI")]
    public GameObject slotUI;
    public GameObject InputNameUI;
    public TMP_InputField newPlayerName;
    public GameObject[] slots;
    private TMP_Text[][] slotText = new TMP_Text[3][];

    private bool[] savefile = new bool[3];
    private string changeSceneName;
    private float time = 0f;

    private bool isNewGame = false;
    private bool isCreate = false;

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            slotText[i] = slots[i].GetComponentsInChildren<TMP_Text>();
        }

        // 슬롯별로 저장된 데이터가 존재하는지 판단.
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(SaveSystem.path + $"{i}"))// 데이터가 있는 경우
            {
                savefile[i] = true;
                SaveSystem.SlotDataLoad(i);

                slotText[i][0].text = SaveSystem.slotData.Name;
                slotText[i][1].text = SaveSystem.slotData.Gold.ToString() + " G";
            }
            else// 데이터가 없는 경우
            {
                slotText[i][0].text = "비어있음";
                slotText[i][1].text = "0 G";
            }
        }

        SaveSystem.DataClear();
    }

    public void NewGameButton(bool isNew)
    {
        isNewGame = isNew;
        slotUI.SetActive(true);
    }

    public void Slot(int number)
    {
        SaveSystem.nowSlot = number;

        if (!isNewGame)//newGame이 아니라면
        {
            if (!savefile[number])
            {
                isCreate = true;
                CreatSlot();
                return;
            }
            SaveSystem.Load();
            GameSceneChange();
        }
        else if (isNewGame) //데이터가 없다면
        {
            isCreate = true;
            CreatSlot();
        }
    }

    public void CreatSlot()	// 플레이어 닉네임 입력 UI를 활성화하는 메소드
    {
        InputNameUI.gameObject.SetActive(true);

        //슬롯에서 적은 이름이 newPlayerName에들어가기
        //슬롯에서 엔터치면 GameSceneChange()부르기
    }

    public void GameSceneChange()
    {
        //print(newPlayerName.text);

        changeSceneName = "2.OutDoor";
        StartCoroutine("LoadingScene");
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveSystem.Save(newPlayerName.text, true);
    }

    private IEnumerator LoadingScene()
    {
        yield return StartCoroutine("SceneChangeFadeIn");

        loadingSliderObj.SetActive(true);

        if (isCreate == true)
            SceneManager.sceneLoaded += SceneLoaded;

        AsyncOperation loading = SceneManager.LoadSceneAsync(changeSceneName);

        while (!loading.isDone) //씬 로딩 완료시 while문이 나가짐
        {
            if (loading.progress >= 0.9f)
                loadingSlider.value = 1f;
            else
                loadingSlider.value = loading.progress;

            yield return null;
        }

        if (savefile[SaveSystem.nowSlot] == false)
            SceneManager.sceneLoaded -= SceneLoaded;

        yield return StartCoroutine("SceneChangeFadeOut");
    }
    private IEnumerator SceneChangeFadeIn()
    {
        fadeImage.gameObject.SetActive(true);

        Color alpha = fadeImage.color;
        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / 1;
            alpha.a = Mathf.Lerp(0, 1, time);
            fadeImage.color = alpha;
            yield return null;
        }

        time = 0f;
    }
    public IEnumerator SceneChangeFadeOut()
    {
        Color alpha = fadeImage.color;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / 1;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

}
