using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed; // ���� �̵� �ӵ�
    private Vector3 direction; // �Ÿ�
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
        // 1. �÷��̾ �ٶ󺸴� ���� ���ϱ�
        direction = (player.transform.position - this.transform.position);
        // �÷��̾ �����Ÿ� �ۿ� ������?
        if(direction.magnitude > 2)
        {
            // 2. ��ֺ��ͷ� ��ȯ�ϱ�
            direction.Normalize(); 
            //normalized�� ���⺤�Ͱ� �ȹٲ�
            //normalize�� ���⺤�ͷ� �ٲ�
            this.transform.position += direction * moveSpeed * Time.deltaTime; // �ӵ��� �������� ���ض�
        }
    }
}
