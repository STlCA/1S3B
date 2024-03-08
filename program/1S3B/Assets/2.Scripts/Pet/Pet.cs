using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public float Pet_Speed; // ���� �̵� �ӵ�
    public Transform Target_Player; // �÷��̾��� ��ġ
    //public float Distance; // �Ÿ�

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
        transform.LookAt(Target_Player); // �÷��̾ �ٶ󺸰� ��

        // 1.�÷��̾ ��� ���󰥰���
        // -�÷��̾�� ���� �Ÿ� �̻� �־����� ��������� ��

        // 2.�÷��̾��� ��� ��ġ�ϰ� �� ���ΰ�
        // -�÷��̾��� �ڿ� ��ġ �ϵ��� �ϰ� ����

        
    }
}
