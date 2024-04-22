using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SceneChangeManager : Manager
{
    private Player player;

    public Image fadeImage;

    private GameObject startCam;
    private GameObject endCam;

    private Vector3 playerPos;
    private string changeSceneName;

    private float fadeTime = 1f;
    private float waitTime = 1f;
    private float time = 0f;

    public Action<bool> mapChangeAction;

    //public delegate void OnChangeDel(bool value);
    //public event OnChangeDel OnChangeEvent;
    //public void CallChangeEvent(bool value)
    //{
    //    isMapChange = value;
    //    OnChangeEvent?.Invoke(value);
    //}

    private void Start()
    {
        player = gameManager.Player;
    }

    public void MapChangeSetting(GameObject startCam, GameObject endCam, float fadeTime, float waitTime)
    {
        this.startCam = startCam;
        this.endCam = endCam;
        this.fadeTime = fadeTime;
        this.waitTime = waitTime;

        StartCoroutine("MapChange");
    }

    public void FadeFlowStart(float fadeTime, float waitTime)
    {
        this.fadeTime = fadeTime;
        this.waitTime = waitTime;

        StartCoroutine("FadeFlow");
    }

    /*    public IEnumerator FadeIn()//가려지게
    {
        fadeImage.gameObject.SetActive(true);

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }
    }

    public IEnumerator FadeOut()//투명하게
    {
        float fadeCount = 1;

        while (fadeCount >= 0f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeInOut()
    {
        fadeImage.gameObject.SetActive(true);

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        StartCoroutine("FadeOut");
    }*/
    /*    public IEnumerator MapChange()
    {
        fadeImage.gameObject.SetActive(true);

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        PlayerStatus.instance.ChangePosition();
        endCam.SetActive(true);
        startCam.SetActive(false);

        fadeCount = 1;
        while (fadeCount >= 0f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        //CallChangeEvent(false);
        isMapChange = false;
        isReAnim = true;
        fadeImage.gameObject.SetActive(false);
    }*/

    public IEnumerator FadeFlow()
    {
        fadeImage.gameObject.SetActive(true);
        Color alpha = fadeImage.color;
        time = 0f;

        Time.timeScale = 0.0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            fadeImage.color = alpha;
            yield return null;
        }

        time = 0f;

        Time.timeScale = 1f;

        yield return new WaitForSeconds(waitTime);

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator MapChange()
    {
        fadeImage.gameObject.SetActive(true);

        Time.timeScale = 0;

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.02f;
            yield return new WaitForSecondsRealtime(0.0001f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        player.ChangePosition();
        endCam.SetActive(true);
        startCam.SetActive(false);

        Time.timeScale = 1;

        yield return new WaitForSecondsRealtime(waitTime);

        fadeCount = 1;

        while (fadeCount >= 0f)
        {
            fadeCount -= 0.02f;
            yield return new WaitForSecondsRealtime(0.0001f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        fadeImage.gameObject.SetActive(false);

        CallMapChangeEvent(false);
    }

    public void CallMapChangeEvent(bool isChange)
    {
        PlayerState previousState = new();

        if (isChange == true)
        {
            previousState = player.playerState;
            player.PlayerStateChange(PlayerState.MAPCHANGE);
        }
        else
            player.PlayerStateChange(previousState);

        mapChangeAction?.Invoke(isChange);
    }

    public IEnumerator SleepFadeIn()
    {
        fadeTime = 2f;

        fadeImage.gameObject.SetActive(true);

        Color alpha = fadeImage.color;
        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(0, 1, time);
            fadeImage.color = alpha;
            yield return null;
        }

        time = 0f;

        player.animationController.DeathAnimation(false);

        //yield return new WaitForSeconds(waitTime);
    }

    public IEnumerator SleepFadeOut()
    {
        Color alpha = fadeImage.color;

        while (alpha.a > 0f)
        {
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);

        player.PlayerStateChange(PlayerState.IDLE);

        fadeTime = 1f;
    }

    public void SceneChangeSetting(string sceneName,Vector3 playerPos)
    {
        changeSceneName = sceneName;
        this.playerPos = playerPos;

        StartCoroutine("LoadingScene");        
    }

    private IEnumerator LoadingScene()
    {
        yield return StartCoroutine("SceneChangeFadeIn");

        AsyncOperation loading = SceneManager.LoadSceneAsync(changeSceneName);//,LoadSceneMode.Additive        

        while (!loading.isDone) //씬 로딩 완료시 로딩완료시 완료된다.
        {
            yield return null;
        } 

        player.playerPosition = playerPos;
        player.ChangePosition();

        yield return StartCoroutine("SceneChangeFadeOut");
    }
    private IEnumerator SceneChangeFadeIn()
    {
        fadeImage.gameObject.SetActive(true);

        Color alpha = fadeImage.color;
        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeTime;
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
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = alpha;
            yield return null;
        }

        fadeImage.gameObject.SetActive(false);
    }
}
