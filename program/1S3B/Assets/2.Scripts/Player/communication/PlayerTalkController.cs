using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTalkController : MonoBehaviour
{
    [SerializeField] List<ITalk> talks = new List<ITalk>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ITalk talk = collision.GetComponent<ITalk>();
        if(talk != null )
        {
            talks.Add(talk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ITalk talk = collision.GetComponent<ITalk>();
        if (talk != null)
        {
            talks.Remove(talk);
        }
    }

    public ITalk FindNearTalk()
    {
        if (talks.Count == 0)
        {
            return null;
        }

        // ����� ������Ʈ ã��
        return talks[0];
    }


    public ITalk NearTalk()
    {
        ITalk talk = FindNearTalk();
        if(talk != null)
        {
            talk.Talk();
            return talk;
        }

        return null;
    }
}
