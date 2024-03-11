using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus player;

    public Vector3 playerPosition;

    private void Awake()
    {
        if (player == null)
            player = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }


}
