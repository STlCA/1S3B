using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Constants;

public class InteractionObjects : MonoBehaviour
{
    public RecoveryType recoveryType;

    private bool inRecoveryZone = false;
    private int recoveryTime;

    private float tempTime = 0;

    private void Update()
    {
        if (inRecoveryZone == true)
            RecoveryTimeCheck();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRecoveryZone = true;

            if (recoveryType == RecoveryType.Bonfire)
                recoveryTime = 5;
            else if (recoveryType == RecoveryType.Spa)
                recoveryTime = 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRecoveryZone = false;
            tempTime = 0;
        }
    }

    private void RecoveryTimeCheck()
    {
        tempTime += Time.deltaTime;

        if (tempTime >= recoveryTime)
        {
            PlayerStatus.player.EnergyRecovery();
            tempTime = 0;
        }
    }
}
