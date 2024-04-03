using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    private Dictionary<int, string[]> talkData; 
    private Dictionary<int, Sprite> portraitData;
    private Dictionary<int, string> npcName;
    //public Sprite[] portraitArr;

    private void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
    }

    public string GetTalk(int id, int talkIndex)
    {
        if(talkIndex == talkData[id].Length)
        {
            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }

    /*public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }*/

    public string GetNpcName(int id)
    {
        npcName.Add(1000, "¿œπ›¿Œ");

        return npcName[id];
    }
   
}
