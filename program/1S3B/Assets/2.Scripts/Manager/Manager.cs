using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    protected GameManager gameManager;

    public virtual void Init(GameManager gm)
    {
        gameManager = gm;
    }

    public virtual void CutParent()
    { 
        transform.parent = null;
    }
}
