using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;
using static UnityEngine.Rendering.ReloadAttribute;

public class AnimationController : AnimationBase
{
    private Vector2 saveDirection = Vector2.zero;
    private bool oneTimeSave = false;

    //public GameObject scmGo;
    //private SceneChangeManager sceneChangeManager;

    private void Start()
    {
        controller.OnMoveEvent += MoveAnimation;
        controller.OnClickEvent += UseAnimation;

        //animator[1].enabled = false;
        //sceneChangeManager = scmGo.GetComponent<SceneChangeManager>();
        //sceneChangeManager.OnChangeEvent += StopAnimation;
    }

    private void Update()
    {
        if (GameManager.Instance.sceneChangeManager.isMapChange == true)
            StopAnimation(true);

        if (GameManager.Instance.sceneChangeManager.isMapChange == false && GameManager.Instance.sceneChangeManager.isReAnim == true)
            StopAnimation(false);
    }

    public void MoveAnimation(Vector2 direction)
    {
        if (animator[0].GetBool("isStart") == false)
        {
            foreach (var anim in animator)
            {
                anim.SetBool("isStart", true);
            }
        }

        if (direction.magnitude <= 0f)
        {
            foreach (var anim in animator)
            {
                anim.SetFloat("saveX", animator[0].GetFloat("inputX"));
                anim.SetFloat("saveY", animator[0].GetFloat("inputY"));
            }       
        }

        foreach(var anim in animator)
        {
            anim.SetFloat("inputX", direction.x);
            anim.SetFloat("inputY", direction.y);

            anim.SetBool("isWalking", direction.magnitude > 0f);
        }
    }

    public void StopAnimation(bool value)
    {
        if (value == true)
        {
            if (oneTimeSave == false)
            {
                saveDirection.x = animator[0].GetFloat("inputX");
                saveDirection.y = animator[0].GetFloat("inputY");

                oneTimeSave = true;
            }

            foreach (var anim in animator)
            {
                anim.SetBool("isWalking", false);

                anim.speed = 0;

                anim.SetFloat("saveX", saveDirection.x);
                anim.SetFloat("saveY", saveDirection.y);
            }         
        }
        else
        {
            GameManager.Instance.sceneChangeManager.isReAnim = false;
            oneTimeSave = false;

            foreach (var anim in animator)
            {
                anim.speed = 1;
            }

            if (animator[0].GetFloat("inputX") != 0 || animator[0].GetFloat("inputY") != 0)
            {
                MoveAnimation(saveDirection);
            }
        }
    }

    public void UseAnimation(PlayerEquipmentType equipmentType)
    {
        if (animator[0].GetFloat("saveX") == 0 && animator[0].GetFloat("saveY") == 0)
        {
            foreach (var anim in animator)
            {
                anim.SetBool("isStart", true);
            }
        }        

        switch (equipmentType)
        {
            case PlayerEquipmentType.PickUp:                
                foreach(var anim in animator)
                {
                    anim.SetTrigger("usePickUp");
                }
                //animator[0].SetTrigger("usePickUp");
                //animator[1].SetTrigger("usePickUp");
                break;
            case PlayerEquipmentType.Hoe:
                foreach (var anim in animator)
                {
                    anim.SetTrigger("useHoe");
                }
                //animator[0].SetTrigger("useHoe");
                //animator[1].SetTrigger("useHoe");
                break;
            case PlayerEquipmentType.Water:
                foreach (var anim in animator)
                {
                    anim.SetTrigger("useWater");
                }
                //animator[0].SetTrigger("useWater");
                //animator[1].SetTrigger("useWater");
                break;
            case PlayerEquipmentType.Axe:
            case PlayerEquipmentType.PickAxe:
            case PlayerEquipmentType.Sword:
            case PlayerEquipmentType.FishingRod:
                break;
        }
    }

    public void DeathAnimation(bool value)
    {
        animator[0].SetBool("isDeath", value);
        animator[1].SetBool("isDeath", value);
    }
}
