using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Plant
{
    public int ID;
    public string Name;
    public int GrowthTime;//한단계가 자라는 시간
    public int GrowthStage;// 작물 성장 단계
    public int NumberOfHarvest;// 수확횟수
    public int ProductPerHarvest;// 수확당 수확갯수
    public int StageAfterHarvest;// 수확후 작물단계
    public int DeathTimer;
    public string SpritePath;
}

public class PlantInstance
{
    int no;
    public Plant plant;
}

[System.Serializable]
public class PlantsDatabase : MonoBehaviour
{
    public List<Plant> PlantsData;        // �̸��� �߿�!!
    public Dictionary<int, Plant> plantDic = new();

    public void Initialize()
    {
        foreach (Plant item in PlantsData)
        {
            plantDic.Add(item.ID, item);
        }
    }

    public Plant GetItemByKey(int id)
    {
        if (plantDic.ContainsKey(id))
            return plantDic[id];

        return null;
    }
}
