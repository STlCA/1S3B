using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed; // ���� �̵� �ӵ�
    private Vector3 direction; // �Ÿ�
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
        // 1. �÷��̾ �ٶ󺸴� ���� ���ϱ�
        direction = (player.transform.position - this.transform.position);
        
        // �÷��̾ �����Ÿ� �ۿ� ������?
        if(direction.magnitude > 2)
        {
            if (direction.magnitude > 10f)
            {
                transform.position = player.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle;
            }

            // 2. ��ֺ��ͷ� ��ȯ�ϱ�
            direction.Normalize(); 
            //normalized�� ���⺤�Ͱ� �ȹٲ�
            //normalize�� ���⺤�ͷ� �ٲ�
            this.transform.position += direction * moveSpeed * Time.deltaTime; // �ӵ��� �������� ���ض�
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
