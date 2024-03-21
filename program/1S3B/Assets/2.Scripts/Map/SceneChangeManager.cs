using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SceneChangeManager : MonoBehaviour
{
    public Image fadeImage;

    private void Start()
    {
        if (GameManager.Instance.sceneChangeManager == null)
            GameManager.Instance.sceneChangeManager = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("여기");

        if (collision.gameObject.tag.Contains("Player"))
        {
            if (gameObject.tag.Contains("Town"))
            {
                PlayerStatus.player.playerPosition = new Vector3(25f, 0f, 0f);                
                StartCoroutine("MapChange");
                Time.timeScale = 0.0f;

            }
            else if (gameObject.tag.Contains("Farm"))
            {
                PlayerStatus.player.playerPosition = new Vector3(12f, 0f, 0f);
                StartCoroutine("MapChange");
                Time.timeScale = 0.0f;
            }
        }
    }
    
    public void ChangePosition()
    {       
        PlayerStatus.player.transform.position = PlayerStatus.player.playerPosition;
    }

    public IEnumerator FadeIn()//알파값높이기
    {
        fadeImage.gameObject.SetActive(true);

        float fadeCount = 0;
        while(fadeCount < 1.0f)
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

        Time.timeScale = 1.0f;

        fadeImage.gameObject.SetActive(false);
    }

    public IEnumerator MapChange()//캐릭터위치변경포함
    {
        fadeImage.gameObject.SetActive(true);

        float fadeCount = 0;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSecondsRealtime(0.005f);
            fadeImage.color = new Color(0, 0, 0, fadeCount);
        }

        //StartCoroutine("FadeIn");
        //yield return new AsyncOperation();

        ChangePosition();

        StartCoroutine("FadeOut");
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
