using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    //waypoint�� �����;���
    [SerializeField] private WayPoint wayPoint;

    // �Ϸ� ���۵ǰ� npc�� �̵��� �ϴ� ���°� ������
    // waypoint�� �����ͼ�, npcmovestate�� ������
    // �Ϸ� �����ų� �������� �����ϸ� �ʱ�ȭ�ϰ� ������ �̵��ϴ� ����

    private void Awake()
    {
        wayPoint = GetComponent<WayPoint>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Call It a Dat = �ϰ��� ��ġ��
    private void CallItaDay()
    {
        // �ʱ�ȭ�� ��Ŵ
    }

    
}
