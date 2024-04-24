using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct SaveData
{
    public PlayerSaveData PlayerData;
    public DaySaveData TimeSaveData;
    public SaveData[] ScenesData;
}

[System.Serializable]
public struct SceneSaveData
{
    public string SceneName;
    public SaveTileData TerrainData;
}

public class SaveSystem : MonoBehaviour
{
    public static PlayerSaveData slotData;
    public static string path; // 경로
    public static int nowSlot; // 현재 슬롯번호

    private static SaveData SaveData = new();
    private static Dictionary<string, SceneSaveData> s_ScenesDataLookup = new Dictionary<string, SceneSaveData>();

    private void Awake()
    {
        path = Application.persistentDataPath + "/save";	// 경로 지정
        print(path);
    }

    public static void SlotDataLoad()
    {
        string data = File.ReadAllText(path + nowSlot.ToString());
        SaveData = JsonUtility.FromJson<SaveData>(data);
        slotData = SaveData.PlayerData;
    }

    public static void Save()
    {
        GameManager.Instance.Player.Save(ref SaveData.PlayerData);
        GameManager.Instance.DayCycleHandler.Save(ref SaveData.TimeSaveData);
        //SaveSceneData();

        string data = JsonUtility.ToJson(SaveData);
        File.WriteAllText(path + nowSlot.ToString(), data);
    }

    public static void Load()
    {
        string data = File.ReadAllText(path + nowSlot.ToString());
        SaveData = JsonUtility.FromJson<SaveData>(data);

        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private static void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.Player.Load(SaveData.PlayerData);
        GameManager.Instance.DayCycleHandler.Load(SaveData.TimeSaveData);
        //LoadSceneData();

        SceneManager.sceneLoaded -= SceneLoaded;
    }

    // public static void SaveSceneData()
    // {
    //     if (GameManager.Instance.TileManager != null)
    //     {
    //         var sceneName = GameManager.Instance.LoadedSceneData.UniqueSceneName;
    //         var data = new SaveTileData();
    //         GameManager.Instance.TileManager.Save(ref data);
    //
    //         s_ScenesDataLookup[sceneName] = new SceneSaveData()
    //         {
    //             SceneName = sceneName,
    //             TerrainData = data
    //         };
    //     }
    // }

    // public static void LoadSceneData()
    // {
    //     if (s_ScenesDataLookup.TryGetValue(GameManager.Instance.LoadedSceneData.UniqueSceneName, out var data))
    //     {
    //         GameManager.Instance.TileManager.Load(data.TerrainData);
    //     }
    // }
    //
    public static void DataClear()
    {
        nowSlot = -1;
        SaveData = new();
        slotData = new();
    }
}
