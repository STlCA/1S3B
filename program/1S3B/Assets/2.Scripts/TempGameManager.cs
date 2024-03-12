using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameManager : MonoBehaviour
{
    public static TempGameManager instance;

    public TileManager tileManager { get; set; }//물어보기


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
