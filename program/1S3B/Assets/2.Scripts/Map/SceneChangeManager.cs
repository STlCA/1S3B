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

    private void Start()
    {
        GameManager.Instance.sceneChangeManager = this;
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

        isMapChange = false;
        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator FadeIn()//알파값높이기
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

    public IEnumerator FadeOut()//알파값낮추기
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

}
