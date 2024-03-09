using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SceneChangeManager : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("여기");

        //string place = collision.gameObject.tag;
        //SceneManager.LoadScene(place);

        if (collision.gameObject.tag.Contains("Player"))
        {
            if (gameObject.tag.Contains("Town"))//나중에 지우기//씬이름을 string으로 받아서 태그랑합쳐서
            {
                PlayerStatus.player.playerPosition = new Vector3(-12f, 0f, 0f);
                SceneManager.LoadScene("JSJ2");
                ChangePosition();
            }
            else if (gameObject.tag.Contains("Farm"))
            {
                PlayerStatus.player.playerPosition = new Vector3(12f, 0f, 0f);                
                SceneManager.LoadScene("JSJ1");
                ChangePosition();
            }
        }


    }
    public void ChangePosition()
    {       
        PlayerStatus.player.transform.position = PlayerStatus.player.playerPosition;
    }
}
