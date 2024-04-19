using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[System.Serializable]
public class TalkInfo
{
    public int Id;
    public List<string> npcDialogue;
}

[System.Serializable]
public class TalksDatabese
{
    public List<TalkInfo> TalkData;
    public Dictionary<int, TalkInfo> TalkDic = new();

    public void Initialize()
    {
        foreach (TalkInfo TalksInfo in TalkData)
        {
            TalkDic.Add(TalksInfo.Id, TalksInfo);
        }
    }

    public TalkInfo GetTalkByKey(int id)
    {
        if (TalkDic.ContainsKey(id))
            return TalkDic[id];

        return null;
    }
}
