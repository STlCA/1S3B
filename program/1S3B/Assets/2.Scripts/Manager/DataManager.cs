using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class DataManager : Manager { 

    public CropDatabase cropDatabase;
    public ItemDatabase itemDatabase;

    void Awake()
    {
        CropAwake();
        ItemAwake();
    }

    private void CropAwake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/Crops_Data");
        if (jsonFile != null)
        {
            string json = jsonFile.text;

            cropDatabase = JsonUtility.FromJson<CropDatabase>(json);
            cropDatabase.Initialize();
/*
            //int itemKeyToFind = 1001;
            //Crop foundPlant = cropDatabase.GetItemByKey(itemKeyToFind);
            //
            //if (foundPlant != null)
            //{
            //    Debug.Log("Crop Name: " + foundPlant.Name);
            //    Debug.Log("Crop ID: " + foundPlant.ID);
            //    Debug.Log("Crop GrowthTime: " + foundPlant.GrowthTime);
            //    Debug.Log("Crop GrowthStage: " + foundPlant.AllGrowthStage);
            //    Debug.Log("Crop ProductPerHarvest: " + foundPlant.ProductPerHarvest);
            //    Debug.Log("Crop StageAfterHarvest: " + foundPlant.StageAfterHarvest);
            //    Debug.Log("Crop DeathTimer: " + foundPlant.DeathTimer);
            //}
            //else
            //{
            //    Debug.Log("Item with key " + itemKeyToFind + " not found.");
            //}*/
        }
        else
        {
            Debug.LogError("Failed to load cropDatabase.json");
        }
    }
    
    private void ItemAwake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/Item_Data");
        if (jsonFile != null)
        {
            string json = jsonFile.text;

            itemDatabase = JsonUtility.FromJson<ItemDatabase>(json);
            itemDatabase.Initialize();
        }
        else
        {
            Debug.LogError("Failed to load itemDatabase.json");
        }
    }
}
