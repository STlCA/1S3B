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
    private GameManager gameManager;
    private SceneChangeManager sceneChangeManager;
    private Player player;
    private PlayerInputController playerInputController;

    private Vector2 saveDirection = Vector2.zero;
    private bool oneTimeSave = false;

    private PlayerState previousState;

    //public GameObject scmGo;
    //private SceneChangeManager SceneChangeManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        sceneChangeManager = gameManager.SceneChangeManager;
        player = gameManager.Player;
        playerInputController = GetComponent<PlayerInputController>();

        controller.OnMoveEvent += MoveAnimation;
        controller.OnClickEvent += UseAnimation;
        sceneChangeManager.mapChangeAction += StopAnimation;
    }

    //private void Update()
    //{
    //    if (GameManager.Instance.SceneChangeManager.isMapChange == true)
    //        StopAnimation(true);
    //
    //    if (GameManager.Instance.SceneChangeManager.isMapChange == false && GameManager.Instance.SceneChangeManager.isReAnim == true)
    //        StopAnimation(false);
    //}

    public void MoveAnimation(Vector2 direction)
    {
        if (animator[0].GetBool(ConstantsString.IsStart) == false)
        {
            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsStart, true);
            }
        }

        if (direction.magnitude <= 0f)
        {
            foreach (var anim in animator)
            {
                anim.SetFloat(ConstantsString.SaveX, animator[0].GetFloat(ConstantsString.InputX));
                anim.SetFloat(ConstantsString.SaveY, animator[0].GetFloat(ConstantsString.InputY));
            }       
        }

        foreach(var anim in animator)
        {
            anim.SetFloat(ConstantsString.InputX, direction.x);
            anim.SetFloat(ConstantsString.InputY, direction.y);

            anim.SetBool(ConstantsString.IsWalking, direction.magnitude > 0f);
        }
    }

    public void StopAnimation(bool value)
    {
        if (value == true)
        {
            if (oneTimeSave == false)
            {
                saveDirection.x = animator[0].GetFloat(ConstantsString.InputX);
                saveDirection.y = animator[0].GetFloat(ConstantsString.InputY);

                oneTimeSave = true;
            }

            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsWalking, false);

                anim.speed = 0;

                anim.SetFloat(ConstantsString.SaveX, saveDirection.x);
                anim.SetFloat(ConstantsString.SaveY, saveDirection.y);
            }         
        }
        else if(value == false) 
        {
            oneTimeSave = false;

            foreach (var anim in animator)
            {
                anim.speed = 1;
            }

            if (animator[0].GetFloat(ConstantsString.InputX) != 0 || animator[0].GetFloat(ConstantsString.InputY) != 0)
                MoveAnimation(saveDirection);
        }
    }

    public void UseAnimation(PlayerEquipmentType equipmentType, Vector2 pos)
    {
        if (animator[0].GetFloat(ConstantsString.SaveX) == 0 && animator[0].GetFloat(ConstantsString.SaveY) == 0)
        {
            foreach (var anim in animator)
            {
                anim.SetBool(ConstantsString.IsStart, true);
            }
        }

        foreach (var anim in animator)
        {
            anim.SetFloat(ConstantsString.SaveX, pos.x);
            anim.SetFloat(ConstantsString.SaveY, pos.y);
        }

        foreach (var anim in animator)
        {
            switch (equipmentType)
            {
                case PlayerEquipmentType.PickUp:
                    anim.SetTrigger("usePickUp");
                    break;
                case PlayerEquipmentType.Hoe:
                    anim.SetTrigger("useHoe");
                    break;
                case PlayerEquipmentType.Water:
                    anim.SetTrigger("useWater");
                    break;
                case PlayerEquipmentType.Axe:
                case PlayerEquipmentType.PickAxe:
                case PlayerEquipmentType.Sword:
                case PlayerEquipmentType.FishingRod:
                    break;
            }
        }

        //StartCoroutine("StateDelay");
    }

    //IEnumerator StateDelay()
    //{
    //    yield return new WaitForSeconds(0.1f);
    //
    //    float curAnimationTime = animator[0].GetCurrentAnimatorStateInfo(0).length;
    //
    //    yield return new WaitForSeconds(curAnimationTime);
    //
    //    playerInputController.UseAnimEnd();
    //}


    public void DeathAnimation(bool value)
    {
        foreach (var anim in animator)
        {
            anim.SetBool(ConstantsString.IsDeath, value);
        }        
    }
}
