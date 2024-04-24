using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed; // 펫의 이동 속도
    private Vector3 direction; // 거리
    private Rigidbody2D rigid;
    public Animator animator;
    [SerializeField] private SpriteRenderer petRenderer;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        SortingOrderChange();
    }

    private void Move()
    { 
        // 1. 플레이어를 바라보는 벡터 구하기
        direction = (player.transform.position - this.transform.position);
        
        // 플레이어가 일정거리 밖에 있으면?
        if(direction.magnitude > 2)
        {
            if (direction.magnitude > 10f)
            {
                transform.position = player.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle;
            }

            // 2. 노멀벡터로 변환하기
            direction.Normalize(); 
            //normalized는 방향벡터가 안바뀜
            //normalize는 방향벡터로 바뀜
            this.transform.position += direction * moveSpeed * Time.deltaTime; // 속도는 마지막에 곱해라
            animator.SetBool("IsWalk", true);
        }
        else
        {
            animator.SetBool("IsWalk", false);
        }
    }

    private void Rotate()
    {
        direction = (player.transform.position - this.transform.position);
        float rotatePet = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        petRenderer.flipX = Mathf.Abs(rotatePet) > 90f;
    }

    private void SortingOrderChange()
    {
        petRenderer.sortingOrder = (int)(transform.position.y * 1000 * -1);
    }
}
