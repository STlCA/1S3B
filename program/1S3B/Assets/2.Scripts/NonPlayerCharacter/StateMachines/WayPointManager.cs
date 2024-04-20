using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    public static WayPointManager instance;

    WayPoint[] wayPoints = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        wayPoints = GetComponentsInChildren<WayPoint>();
    }

    public WayPoint GetRandomWayPoint()
    {
        if(wayPoints == null || wayPoints.Length == 0) 
        {
            return null;
        }

        int idx = Random.Range(0, wayPoints.Length);
        Debug.Log("idx" + idx);
        return wayPoints[idx];
    }
}
