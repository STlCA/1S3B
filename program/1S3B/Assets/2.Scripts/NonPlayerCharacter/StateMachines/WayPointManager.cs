using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    //waypoint를 가져와야함
    [SerializeField] private WayPoint wayPoint;

    // 하루 시작되고 npc가 이동을 하는 상태가 나오면
    // waypoint를 가져와서, npcmovestate로 보내줌
    // 하루 끝나거나 목적지에 도착하면 초기화하고 다음날 이동하는 상태

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

    //Call It a Dat = 일과를 마치다
    private void CallItaDay()
    {
        // 초기화를 시킴
    }

    
}
