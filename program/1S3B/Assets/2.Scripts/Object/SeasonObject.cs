using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SeasonObject : MonoBehaviour
{
    private GameManager gameManager;
    private DayCycleHandler dayCycleHandler;

    private SpriteResolver myResolver;

    private void Start()
    {
        gameManager = GameManager.Instance;
        dayCycleHandler = gameManager.DayCycleHandler;

        myResolver = GetComponent<SpriteResolver>();

        dayCycleHandler.changeSeasonAction += MapObjectSpriteChange;
    }

    private void MapObjectSpriteChange(Season season)
    {
        string myType = myResolver.GetCategory();

        myResolver.SetCategoryAndLabel(myType, season.ToString());
    }
}
