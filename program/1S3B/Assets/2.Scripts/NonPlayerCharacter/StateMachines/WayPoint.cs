using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private int pointCount = 0;
    private List<Transform> points = new List<Transform>();


    private void Awake()
    {
        pointCount = transform.childCount;
        for (int i = 0; i < pointCount; i++)
        {
            points.Add(transform.GetChild(i));
        }
    }

    public Transform GetPoint(int i)
    {
        if (pointCount <= i)
            i %= pointCount;

        return points[i];
    }

    public int GetNearIndex(Transform target)
    {
        float minDist = Mathf.Infinity; // Mathf.Infinity = 양의 무한대
        int minIdx = -1;
        for (int i = 0;i < points.Count;i++)
        {
            Transform transformPoints = points[i];
            float distance = Vector3.Distance(transformPoints.position, target.position);
            if (distance < minDist)
            {
                minDist = distance;
                minIdx = i;
            }
        }
        return minIdx;
    }
}
