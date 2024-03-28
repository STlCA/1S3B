using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Constants;
using UnityEngine.Rendering.Universal;
using UnityEditor.Rendering;

public class RecoveryObjects : MonoBehaviour
{
    public RecoveryType recoveryType;

    private bool inRecoveryZone = false;
    private float recoveryTime;

    private float tempTime = 0;

    public Light2D bonfire = null;

    private float lightTime = 0;
    private bool lightUp = false;


    private void Update()
    {
        if (inRecoveryZone == true)
            RecoveryTimeCheck();

        if (lightTime >= 5)
            Destroy(gameObject);

        if (recoveryType == RecoveryType.Bonfire)
            LightTimeCheck();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inRecoveryZone = true;

            if (recoveryType == RecoveryType.Bonfire)
                recoveryTime = 5;
            else if (recoveryType == RecoveryType.Spa)
                recoveryTime = 0.5f;
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
            PlayerStatus.instance.EnergyRecovery();
            tempTime = 0;
        }
    }

    private void LightTimeCheck()
    {
        lightTime += Time.deltaTime;

        if (lightUp == true && bonfire.pointLightOuterRadius < 3.5f)
            bonfire.pointLightOuterRadius += Time.deltaTime;
        else if (lightUp == true && bonfire.pointLightOuterRadius >= 3.5f)
            lightUp = false;
        else if (lightUp == false && bonfire.pointLightOuterRadius >= 2.8f)
            bonfire.pointLightOuterRadius -= Time.deltaTime;
        else
            lightUp = true;
    }
}
