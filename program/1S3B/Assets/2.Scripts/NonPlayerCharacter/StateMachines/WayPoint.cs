using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    int pointCount = 0;
    List<Transform> points = new List<Transform>();


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
            i -= pointCount;

        return points[i];
    }

    public int GetNearIndex(Transform target)
    {
        float minDist = Mathf.Infinity;
        int minIdx = -1;
        for (int i = 0;i < points.Count;i++) 
        {
            Transform t = points[i];
            float d = Vector3.Distance(t.position, target.position);
            if (d < minDist)
            {
                minDist = d;
                minIdx = i;
            }
        }

        return minIdx;
    }
}
