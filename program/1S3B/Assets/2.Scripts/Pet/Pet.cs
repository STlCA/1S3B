using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed; // 펫의 이동 속도
    private Vector3 direction; // 거리
    private Rigidbody2D rigid; 

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    { 
        // 1. 플레이어를 바라보는 벡터 구하기
        direction = (player.transform.position - this.transform.position);
        // 플레이어가 일정거리 밖에 있으면?
        if(direction.magnitude > 2)
        {
            // 2. 노멀벡터로 변환하기
            direction.Normalize(); 
            //normalized는 방향벡터가 안바뀜
            //normalize는 방향벡터로 바뀜
            this.transform.position += direction * moveSpeed * Time.deltaTime; // 속도는 마지막에 곱해라
        }
    }
}
