using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public float Pet_Speed; // 펫의 이동 속도
    public Transform Target_Player; // 플레이어의 위치
    //public float Distance; // 거리

    private void Awake()
    {
        Target_Player = GetComponent<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Pet_Move();
    }

    public void Pet_Move()
    {
        transform.LookAt(Target_Player); // 플레이어를 바라보게 함

        // 1.플레이어를 어떻게 따라갈건지
        // -플레이어와 일정 거리 이상 멀어지면 따라오도록 함

        // 2.플레이어의 어디에 위치하게 둘 것인가
        // -플레이어의 뒤에 위치 하도록 하고 싶음

        
    }
}
