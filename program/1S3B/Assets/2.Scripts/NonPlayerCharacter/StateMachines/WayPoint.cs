using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private Npc npc;
    private NpcStateMachine npcSateMachine;
    private int watPointIndex = 0;

    private void Start()
    {
        npc = GetComponent<Npc>();
        npcSateMachine = GetComponent<NpcStateMachine>();
    }

    public void Update()
    {
        PointsMove();
    }

    private void PointsMove()
    {
        npc.transform.position = Vector2.MoveTowards(npc.transform.position, npc.wayPoints[watPointIndex].transform.position, 3 * Time.deltaTime);

        //npc.transform.position = Vector2.MoveTowards(npc.transform.position, npc.wayPoints[watPointIndex].transform.position, npcSateMachine.movementSpeedModifier * Time.deltaTime);

        if(npc.transform.position == npc.wayPoints[watPointIndex].transform.position)
        {
            watPointIndex++;
        }

        if(watPointIndex == npc.wayPoints.Length)
        {
            watPointIndex = 0;
        }
    }
}
