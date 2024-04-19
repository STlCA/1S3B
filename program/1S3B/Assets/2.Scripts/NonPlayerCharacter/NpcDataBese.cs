using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NpcInfo
{
    public int npcId;
    public string npcName;
    public string npcSprite_path;
    public int talk_Id;
}

[System.Serializable]
public class NpcDataBese
{
    public List<NpcInfo> NpcData;
    public Dictionary<int, NpcInfo> NpcDic = new();

    public void Initialize()
    {
        foreach (NpcInfo npcInfo in NpcData)
        {
            NpcDic.Add(npcInfo.npcId, npcInfo);
        }
    }

    public NpcInfo GetNpcByKey(int id)
    {
        if (NpcDic.ContainsKey(id))
            return NpcDic[id];

        return null;
    }
}





