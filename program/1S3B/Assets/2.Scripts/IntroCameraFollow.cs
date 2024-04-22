using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraFollow : MonoBehaviour
{
    //public GameObject maxPositionObj;
    //public GameObject minPositionObj;
    public float followSpeed = 1f;
    private float maxYPosition = 13.5f;
    private float minYPosition = 7.46f;
    private float direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        direction += Time.deltaTime * followSpeed;
        if(direction >= maxYPosition)
        {
            followSpeed *= -1;
            direction = maxYPosition;
        }
        else if(direction <= minYPosition)
        {
            followSpeed *= -1;
            direction = minYPosition;
        }

        transform.position = new Vector3(-4.54f,direction,-10f);
    }
}
