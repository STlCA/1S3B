using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

[System.Serializable]
public class Crop
{
    public int ID;
    public string Name;
    public string Season;
    public int GrowthTime;//총자라는 시간
    public int ReGrowthTime;//수확후 다시 자라는 시간
    public int AllGrowthStage;// 작물 성장 단계
    public int ProductPerHarvest;// 수확당 수확갯수
    public int StageAfterHarvest;// 수확후 작물단계
    public int DeathTimer;
    public string Path;
    public List<string> SpriteName; // sample 111.111.

    public List<Sprite> SpriteList;

    public void Init()
    {
        foreach (string path in SpriteName)
        {
            SpriteList.Add(Resources.Load<Sprite>(Path+path));
        }
    }
}

// 아이템에 대한 정보를 가져온다
public class CropInstance
{
    int no;
    public Crop plant;
}

[System.Serializable]
public class CropDatabase
{
    public List<Crop> CropsData;//이름을 똑같게
    public Dictionary<int, Crop> cropDic = new();

    public void Initialize()
    {
        foreach (Crop plant in CropsData)
        {
            plant.Init();
            cropDic.Add(plant.ID, plant);
        }
    }

    public Crop GetItemByKey(int id)
    {
        if (cropDic.ContainsKey(id))
            return cropDic[id];

        return null;
    }
}
