using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    public Image fadeImage;

    [HideInInspector] public bool isMapChange = false;
    [HideInInspector] public bool isReAnim = false;

    private GameObject startCam;
    private GameObject endCam;

    //public delegate void OnChangeDel(bool value);
    //public event OnChangeDel OnChangeEvent;

    private void Start()
    {
        GameManager.Instance.sceneChangeManager = this;
    }

    //public void CallChangeEvent(bool value)
    //{
    //    isMapChange = value;
    //    OnChangeEvent?.Invoke(value);
    //}

    public IEnumerator FadeIn()//가려지게
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
    }

    public void MapChangeSetting(GameObject startCam, GameObject endCam)
    {
        this.startCam = startCam;
        this.endCam = endCam;

        StartCoroutine("MapChange");
    }

    public IEnumerator MapChange()
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
    }


    public IEnumerator SleepFadeInOut()
    {
        fadeImage.gameObject.SetActive(true);

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        PlayerStatus.instance.animationController.DeathAnimation(false);
        Time.timeScale = 1.0f;

        fadeCount = 1;

        while (fadeCount >= 0f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        fadeImage.gameObject.SetActive(false);
    }

}
