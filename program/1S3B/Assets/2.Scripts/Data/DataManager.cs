using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class DataManager : MonoBehaviour
{
    public PlantsDatabase plantsDatabase;

    void Awake()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JSON/Plants_Data");
        if (jsonFile != null)
        {
            string json = jsonFile.text;

            plantsDatabase = JsonUtility.FromJson<PlantsDatabase>(json);
            plantsDatabase.Initialize();

            int itemKeyToFind = 3;
            Plant foundPlant = plantsDatabase.GetItemByKey(itemKeyToFind);

            if (foundPlant != null)
            {
                Debug.Log("Plant Name: " + foundPlant.Name);
                Debug.Log("Plant ID: " + foundPlant.ID);
                Debug.Log("Plant GrowthTime: " + foundPlant.GrowthTime);
                Debug.Log("Plant GrowthStage: " + foundPlant.GrowthStage);
                Debug.Log("Plant NumberOfHarvest: " + foundPlant.NumberOfHarvest);
                Debug.Log("Plant ProductPerHarvest: " + foundPlant.ProductPerHarvest);
                Debug.Log("Plant StageAfterHarvest: " + foundPlant.StageAfterHarvest);
                Debug.Log("Plant DeathTimer: " + foundPlant.DeathTimer);
                Debug.Log("Plant SpritePath: " + foundPlant.SpritePath);
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
