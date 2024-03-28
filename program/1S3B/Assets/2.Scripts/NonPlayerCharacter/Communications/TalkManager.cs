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
        GenrateData();
    }

    private void GenrateData()
    {
        //talk
        talkData.Add(1000, new string[] { "안녕, 반가워:1", "난 NPC라고 해:1", "너의 이름을 알려줄래?:1", "제 이름은 플레이어 입니다. : 0"});


        //portrait
       // portraitData.Add(1000 + 0, portraitArr[0]); // player img
       // portraitData.Add(1001 + 0, portraitArr[1]); // npc img
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
        npcName.Add(1000, "일반인");

        return npcName[id];
    }
   
}
