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
        Debug.Log("���̵�");

        if (other.gameObject.tag.Contains("Player"))
        {
            if (type == MapTriggerType.FarmToTown)
            {
                PlayerStatus.instance.playerPosition = new Vector3(-88f, -12f, 0f);
                //sceneChangeManager.CallChangeEvent(true);                
                sceneChangeManager.isMapChange = true;
                sceneChangeManager.MapChangeSetting(startCam, endCam);
            }
            else if (type == MapTriggerType.TownToFarm)
            {
                PlayerStatus.instance.playerPosition = new Vector3(-41f, -3.5f, 0f);
                //sceneChangeManager.CallChangeEvent(true);
                sceneChangeManager.isMapChange = true;
                sceneChangeManager.MapChangeSetting(startCam, endCam);
            }
        }
    }
}
