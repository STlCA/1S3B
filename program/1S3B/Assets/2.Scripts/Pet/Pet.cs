using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public GameObject Player;
    public float Pet_Speed; // 펫의 이동 속도
    public float Distance = 2f; // 거리
    //public Transform Target_Player; // 플레이어의 위치

    private Rigidbody2D Pet_rigidbody;

    private void Awake()
    {
        Pet_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Pet_Move();
    }

    public void Pet_Move()
    {

        if (Mathf.Abs(this.transform.position.x - Player.transform.position.x) > Distance)
        {
            this.transform.Translate(new Vector2(-1, 0) * Time.deltaTime * Pet_Speed);
            Pet_Direction_X();
        }

        if (Mathf.Abs(this.transform.position.y - Player.transform.position.y) > Distance)
        {
            this.transform.Translate(new Vector2(0, -1) * Time.deltaTime * Pet_Speed);
            Pet_Direction_Y();
        }
 
    }

    private void Pet_Direction_X()
    {
        if(this.transform.position.x - Player.transform.position.x < 0)
        {
            this.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            this.transform.eulerAngles = new Vector3(0, 0, 0);
        }

    }

    private void Pet_Direction_Y()
    {
        if (this.transform.position.y - Player.transform.position.y < 0)
        {
            this.transform.eulerAngles = new Vector3(180, 0, 0);
        }
        else
        {
            this.transform.eulerAngles = new Vector3(0, 0, 0);
        }

    }
}
