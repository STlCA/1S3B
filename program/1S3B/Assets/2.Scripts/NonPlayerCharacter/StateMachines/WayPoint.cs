using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    // WayPoint의 데이터 관리

    public static Transform[] wayPoints;
    //public GameObject wayPoint1; 
    ///public GameObject wayPoint2;
    //public GameObject wayPoint3;
    //private int wayPointIndex = 0;

    private void Awake()
    {
        //GetRandomWayPoint();
    }

    /*private void GetRandomWayPoint()
    {
        // randomWayPoint를 지우고 요일? 을 넣을 예정
        int randomWayPoint = Random.Range(1, 4);
        Debug.Log(randomWayPoint);
        switch (randomWayPoint)
        {
            case 1:
                wayPoint1.SetActive(true);
                wayPoints = new Transform[wayPoint1.transform.childCount];
                for (int wayPointsChildCount = 0; wayPointsChildCount < wayPoints.Length; wayPointsChildCount++)
                {
                    wayPoints[wayPointsChildCount] = wayPoint1.transform.GetChild(wayPointsChildCount);
                }
                break;

            case 2:
                wayPoint2.SetActive(true);
                wayPoints = new Transform[wayPoint2.transform.childCount];
                for (int wayPointsChildCount = 0; wayPointsChildCount < wayPoints.Length; wayPointsChildCount++)
                {
                    wayPoints[wayPointsChildCount] = wayPoint2.transform.GetChild(wayPointsChildCount);
                }
                break;

            case 3:
                wayPoint3.SetActive(true);
                wayPoints = new Transform[wayPoint3.transform.childCount];
                for (int wayPointsChildCount = 0; wayPointsChildCount < wayPoints.Length; wayPointsChildCount++)
                {
                    wayPoints[wayPointsChildCount] = wayPoint3.transform.GetChild(wayPointsChildCount);
                }
                break;
            default:
                return;
        }
    }*/
}
