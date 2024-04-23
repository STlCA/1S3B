using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    public Image fadeImage;
    public GameObject loadingSliderObj;
    private Slider loadingSlider;

    private string changeSceneName;
    private float time = 0f;

    private void Start()
    {
        loadingSlider = loadingSliderObj.GetComponent<Slider>();
    }

    public void GameSceneChange()
    {
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

    private IEnumerator LoadingScene()
    {
        yield return StartCoroutine("SceneChangeFadeIn");

        AsyncOperation loading = SceneManager.LoadSceneAsync(changeSceneName);//,LoadSceneMode.Additive        

        loadingSliderObj.SetActive(true);

        while (!loading.isDone) //�� �ε� �Ϸ�� �ε��Ϸ�� �Ϸ�ȴ�.
        {
            if (loading.progress >= 0.9f)
                loadingSlider.value = 1f;
            else
                loadingSlider.value = loading.progress;

            yield return null;
        }

        yield return StartCoroutine("SceneChangeFadeOut");
    }
    private IEnumerator SceneChangeFadeIn()
    {
        fadeImage.gameObject.SetActive(true);

        Color alpha = fadeImage.color;
        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / 2;
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
            time += Time.deltaTime / 2;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

}
