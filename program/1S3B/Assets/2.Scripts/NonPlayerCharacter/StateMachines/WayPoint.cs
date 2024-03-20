using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    // WayPoint�� ������ ����

    [HideInInspector] public Transform[] wayPoints;
    public GameObject wayPoint1; 
    public GameObject wayPoint2;
    public GameObject wayPoint3;

    public NpcStateMachine npcStateMachine;

    [HideInInspector] public Transform targetWayPoints;
    private int wayPointIndex = 0;
    //private int wayPointIndex = 0;

    public void GetRandomWayPoint(int randomWayPoint)
    {
        // randomWayPoint�� ����� ����? �� ���� ����
        //int randomWayPoint = Random.Range(1, 4);
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
                break;
        }
    }

    public void PointsMove()
    {
        targetWayPoints = wayPoints[wayPointIndex];

        Vector3 direction = targetWayPoints.position - npcStateMachine._npc.transform.position;
        npcStateMachine._npc.transform.Translate(direction.normalized * npcStateMachine.movementSpeedModifier * Time.deltaTime, Space.World);


        //npc�� ����waypoint�� ��ġ�� ����� ���ٸ�
        if (Vector3.Distance(npcStateMachine._npc.transform.position, targetWayPoints.position) <= 0.4f)
        {
            NextWayPoint();
        }
    }

    private void NextWayPoint()
    {
        if (wayPointIndex >= wayPoints.Length - 1)
        {
            wayPointIndex = -1;
            return;
        }
        wayPointIndex++;
        targetWayPoints = wayPoints[wayPointIndex];
    }
}
