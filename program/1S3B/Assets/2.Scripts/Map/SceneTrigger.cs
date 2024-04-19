using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    private GameManager gameManager;
    private SceneChangeManager sceneChangeManager;

    public SceneChangeType myType;
    private bool canEnter = false;
    private Vector3 playePos = Vector3.zero;

    private void Start()
    {
        gameManager = GameManager.Instance;
        sceneChangeManager = gameManager.SceneChangeManager;

        if (myType == SceneChangeType.Home || myType == SceneChangeType.Shop)
            canEnter = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //LayerMask layer = collision.callbackLayers;
        //if(layer == 1<<LayerMask.NameToLayer("Player") && canEnter == true)
        if (collision.tag == "Player" && canEnter == true)
        {
            switch (myType)
            {
                case SceneChangeType.Home:
                    playePos = new Vector3(10, 10, 0);
                    break;
                case SceneChangeType.Shop:
                    playePos = new Vector3(5, 5, 0);
                    break;
            }

            sceneChangeManager.SceneChangeSetting("3.Indoor", playePos);
        }
    }

}
