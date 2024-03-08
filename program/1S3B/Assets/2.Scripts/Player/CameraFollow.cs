using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 camOffset;
    private void Start()
    {
        camOffset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = target.position + camOffset;
    }
}
