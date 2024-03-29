using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using Cinemachine;

public class MapTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private Player playerStatus;
    private SceneChangeManager sceneChangeManager;

    [Header("Type")]
    public MapTriggerType type;

    [Header("Camera")]
    public GameObject startCam;
    public GameObject endCam;

    private void Start()
    {
        gameManager = GameManager.Instance;
        sceneChangeManager = gameManager.SceneChangeManager;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Contains("Player"))
        {
            switch (type)
            {
                case MapTriggerType.FarmToTown:
                    playerStatus.playerPosition = new Vector3(-88f, -12f, 0f);
                    gameManager.playerMap = PlayerMap.Town;
                    break;
                case MapTriggerType.TownToFarm:
                    playerStatus.playerPosition = new Vector3(-41f, -3.5f, 0f);
                    gameManager.playerMap = PlayerMap.Farm;
                    break;
            }


            //SceneChangeManager.isMapChage = true;
            sceneChangeManager.CallMapChangeEvent(true);
            sceneChangeManager.MapChangeSetting(startCam, endCam,2f,2f);
        }
    }
}
