using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using Cinemachine;

public class MapTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;
    private SceneChangeManager sceneChangeManager;
    private UIManager uiManager;
    private WeatherSystem weatherSystem;
    private PopUpController popUpController;

    [Header("Type")]
    public MapTriggerType type;

    [Header("Camera")]
    public GameObject startCam;
    public GameObject endCam;

    private void Start()
    {
        gameManager = GameManager.Instance;
        sceneChangeManager = gameManager.SceneChangeManager;
        player = gameManager.Player;
        uiManager = gameManager.UIManager;
        weatherSystem = gameManager.WeatherSystem;
        popUpController = gameManager.PopUpController;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            switch (type)
            {
                case MapTriggerType.Farm_FarmToTown:
                    player.playerPosition = new Vector3(-23.5f, 1f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.FarmToTown);
                    break;
                case MapTriggerType.FarmToTown_Town:
                    player.playerPosition = new Vector3(-45.6f, 1f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Town);
                    break;
                case MapTriggerType.Town_FarmToTown:
                    player.playerPosition = new Vector3(-41.5f, 1f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.FarmToTown);
                    break;
                case MapTriggerType.FarmToTown_Farm:
                    player.playerPosition = new Vector3(-19f, 0.7f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Farm);
                    break;
                case MapTriggerType.Farm_ForestRoad:
                    player.playerPosition = new Vector3(-13.95f, 30.6f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.FarmToForest);
                    break;
                case MapTriggerType.ForestRoad_Forest:
                    player.playerPosition = new Vector3(-144f, 25.5f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Forest);
                    break;
                case MapTriggerType.Forest_ForestRoad:
                    player.playerPosition = new Vector3(-57.5f, 30.5f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.FarmToForest);
                    break;
                case MapTriggerType.ForestRoad_Farm:
                    player.playerPosition = new Vector3(-13.95f, 24.4f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Farm);
                    break;
                case MapTriggerType.Forest_BossRoom:
                    player.playerPosition = new Vector3(-195.5f, 9.55f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.BossRoom);
                    break;
                case MapTriggerType.BossRoom_Forest:
                    player.playerPosition = new Vector3(-190f, 9f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Forest);
                    break;
                case MapTriggerType.Town_TownRoad:
                    player.playerPosition = new Vector3(-108f, -4.3f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.TownToForest);
                    break;
                case MapTriggerType.TownRoad_Forest:
                    player.playerPosition = new Vector3(-134.77f, -3.67f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Forest);
                    break;
                case MapTriggerType.Forest_TownRoad:
                    player.playerPosition = new Vector3(-129f, -4.5f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.TownToForest);
                    break;
                case MapTriggerType.TownRoad_Town:
                    player.playerPosition = new Vector3(-102f, -4f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Town);
                    break;
                case MapTriggerType.Town_Beach:
                    player.playerPosition = new Vector3(-75f, -49.5f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Beach);
                    break;
                case MapTriggerType.Beach_Island:
                    player.playerPosition = new Vector3(-88.67f, -109.25f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Island);
                    break;
                case MapTriggerType.Island_Beach:
                    player.playerPosition = new Vector3(-58.19f, -87.36f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Beach);
                    break;
                case MapTriggerType.Beach_BeachR:
                    player.playerPosition = new Vector3(-36.73f, -65.85f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.BeachR);
                    break;
                case MapTriggerType.BeachR_Quarry:
                    player.playerPosition = new Vector3(-6.52f, -99.45f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Quarry);
                    break;
                case MapTriggerType.Quarry_Island:
                    player.playerPosition = new Vector3(-46.09f, -112.96f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Island);
                    break;
                case MapTriggerType.Island_Quarry:
                    player.playerPosition = new Vector3(-37f, -112.82f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Quarry);
                    break;
                case MapTriggerType.Quarry_BeachR:
                    player.playerPosition = new Vector3(0f, -91f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.BeachR);
                    break;
                case MapTriggerType.BeachR_Beach:
                    player.playerPosition = new Vector3(-46f, -65f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Beach);
                    break;
                case MapTriggerType.Beach_BeachL:
                    player.playerPosition = new Vector3(-135.18f, -61.54f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.BeachL);
                    break;
                case MapTriggerType.BeachL_Beach:
                    player.playerPosition = new Vector3(-101.95f, -63.94f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Beach);
                    break;
                case MapTriggerType.BeachL_Forest:
                    player.playerPosition = new Vector3(-164.4f, -41.03f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Forest);
                    break;
                case MapTriggerType.Forest_Beach:
                    player.playerPosition = new Vector3(-164.4f, -46f, 0f);
                    uiManager.MiniMapPosition(PlayerMap.Beach);
                    break;
                case MapTriggerType.Farm_Home:
                    player.playerPosition = new Vector3(54.43f + 290f, 0.26f, 0);
                    uiManager.MiniMapPosition(PlayerMap.Home);
                    weatherSystem.SwitchAllElementsToOutdoor(false);
                    break;
                case MapTriggerType.Home_Farm:
                    player.playerPosition = new Vector3(-0.04f, 5.41f, 0);
                    uiManager.MiniMapPosition(PlayerMap.Farm);
                    weatherSystem.SwitchAllElementsToOutdoor(true);
                    break;
                case MapTriggerType.Town_Shop:
                    player.playerPosition = new Vector3(84.35f + 290f, 1f, 0);
                    uiManager.MiniMapPosition(PlayerMap.Shop);
                    weatherSystem.SwitchAllElementsToOutdoor(false);
                    break;
                case MapTriggerType.Shop_Town:
                    player.playerPosition = new Vector3(-74.49f, 6.94f, 0);
                    uiManager.MiniMapPosition(PlayerMap.Town);
                    weatherSystem.SwitchAllElementsToOutdoor(true);
                    break;
                case MapTriggerType.KeepOut:
                    popUpController.UIOn(uiManager.keepOutInfo);
                    return;
                case MapTriggerType.Beach_Town:
                    player.playerPosition = new Vector3(-73.76f, -42.12f, 0);
                    uiManager.MiniMapPosition(PlayerMap.Town);
                    break;
            }

            sceneChangeManager.CallMapChangeEvent(true);
            sceneChangeManager.MapChangeSetting(startCam, endCam, 0.0001f, 0.1f);
        }
    }
}
