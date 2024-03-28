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
        talkData.Add(1000, new string[] { "�ȳ�, �ݰ���:1", "�� NPC��� ��:1", "���� �̸��� �˷��ٷ�?:1", "�� �̸��� �÷��̾� �Դϴ�. : 0"});


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
        npcName.Add(1000, "�Ϲ���");

        return npcName[id];
    }
   
}
