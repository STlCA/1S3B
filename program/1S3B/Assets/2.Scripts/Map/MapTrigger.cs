using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using Cinemachine;

public class MapTrigger : MonoBehaviour
{
    private SceneChangeManager sceneChangeManager;

    [Header("Type")]
    public MapTriggerType type;

    [Header("Camera")]
    public GameObject startCam;
    public GameObject endCam;

    private void Start()
    {
        sceneChangeManager = GetComponentInParent<SceneChangeManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("∏ ¿Ãµø");

        if (other.gameObject.tag.Contains("Player"))
        {
            if (type == MapTriggerType.FarmToTown)
            {
                PlayerStatus.instance.playerPosition = new Vector3(25f, 0f, 0f);
                //sceneChangeManager.CallChangeEvent(true);                
                sceneChangeManager.isMapChange = true;
                sceneChangeManager.MapChangeSetting(startCam, endCam);
            }
            else if (type == MapTriggerType.TownToFarm)
            {
                PlayerStatus.instance.playerPosition = new Vector3(12f, 0f, 0f);
                //sceneChangeManager.CallChangeEvent(true);
                sceneChangeManager.isMapChange = true;
                sceneChangeManager.MapChangeSetting(startCam, endCam);
            }
        }
    }
}
