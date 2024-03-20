using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    //waypoint�� �����;���
    public WayPoint wayPoint;
   

    // �Ϸ� ���۵ǰ� npc�� �̵��� �ϴ� ���°� ������
    // waypoint�� �����ͼ�, npcmovestate�� ������
    // �Ϸ� �����ų� �������� �����ϸ� �ʱ�ȭ�ϰ� ������ �̵��ϴ� ����

    private void Awake()
    {
        wayPoint = GameObject.FindGameObjectWithTag("WayPoint").GetComponent<WayPoint>();       
    }

    // Start is called before the first frame update
    public void Start()
    {
        // waypoint�� �����;���...
        int randomWayPoint = Random.Range(1, 4);
        wayPoint.GetRandomWayPoint(randomWayPoint);
    }

    public void Update()
    {
        wayPoint.PointsMove();
    }



    //Call It a Dat = �ϰ��� ��ġ��
    private void CallItaDay()
    {
        // �ʱ�ȭ�� ��Ŵ
    }

}
