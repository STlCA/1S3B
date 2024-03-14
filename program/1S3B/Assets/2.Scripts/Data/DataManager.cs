using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class DataManager : MonoBehaviour
{
    public static CropDatabase cropDatabase;

    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/Crops_Data");
        if (jsonFile != null)
        {
            string json = jsonFile.text;

            cropDatabase = JsonUtility.FromJson<CropDatabase>(json);
            cropDatabase.Initialize();

            int itemKeyToFind = 1;
            Crop foundPlant = cropDatabase.GetItemByKey(itemKeyToFind);

            if (foundPlant != null)
            {
                Debug.Log("Crop Name: " + foundPlant.Name);
                Debug.Log("Crop ID: " + foundPlant.ID);
                Debug.Log("Crop GrowthTime: " + foundPlant.GrowthTime);
                Debug.Log("Crop GrowthStage: " + foundPlant.AllGrowthStage);
                Debug.Log("Crop NumberOfHarvest: " + foundPlant.NumberOfHarvest);
                Debug.Log("Crop ProductPerHarvest: " + foundPlant.ProductPerHarvest);
                Debug.Log("Crop StageAfterHarvest: " + foundPlant.StageAfterHarvest);
                Debug.Log("Crop DeathTimer: " + foundPlant.DeathTimer);
            }
            else
            {
                Debug.Log("Item with key " + itemKeyToFind + " not found.");
            }
        }
        else
        {
            Debug.LogError("Failed to load itemDatabase.json");
        }
    }
}
