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

        // ���Ժ��� ����� �����Ͱ� �����ϴ��� �Ǵ�.
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(SaveSystem.path + $"{i}"))// �����Ͱ� �ִ� ���
            {
                savefile[i] = true;
                SaveSystem.SlotDataLoad(i);

                slotText[i][0].text = SaveSystem.slotData.Name;
                slotText[i][1].text = SaveSystem.slotData.Gold.ToString() + " G";
            }
            else// �����Ͱ� ���� ���
            {
                slotText[i][0].text = "�������";
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

        if (!isNewGame)//newGame�� �ƴ϶��
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
        else if (isNewGame) //�����Ͱ� ���ٸ�
        {
            isCreate = true;
            CreatSlot();
        }
    }

    public void CreatSlot()	// �÷��̾� �г��� �Է� UI�� Ȱ��ȭ�ϴ� �޼ҵ�
    {
        InputNameUI.gameObject.SetActive(true);

        //���Կ��� ���� �̸��� newPlayerName������
        //���Կ��� ����ġ�� GameSceneChange()�θ���
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

        while (!loading.isDone) //�� �ε� �Ϸ�� while���� ������
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
