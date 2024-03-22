using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

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
        if (animator[0].GetBool("isStart")==false)
            animator[0].SetBool("isStart", true);

        if (direction.magnitude <= 0f)
        {
            animator[0].SetFloat("saveX", animator[0].GetFloat("inputX"));
            animator[0].SetFloat("saveY", animator[0].GetFloat("inputY"));
        }

        animator[0].SetFloat("inputX", direction.x);
        animator[0].SetFloat("inputY", direction.y);

        animator[0].SetBool("isWalking", direction.magnitude > 0f);
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

            animator[0].SetBool("isWalking", false);

            animator[0].speed = 0;

            animator[0].SetFloat("saveX", saveDirection.x);
            animator[0].SetFloat("saveY", saveDirection.y);
        }
        else
        {
            GameManager.Instance.sceneChangeManager.isReAnim = false;
            oneTimeSave = false;

            animator[0].speed = 1;

            if (animator[0].GetFloat("inputX") != 0 || animator[0].GetFloat("inputY") != 0)
            {
                MoveAnimation(saveDirection);
            }
        }
    }

    public void UseAnimation(int equip)
    {
        if (animator[0].GetFloat("saveX")== 0 && animator[0].GetFloat("saveY") == 0)
        {
            animator[0].SetBool("isStart", true);
            animator[1].SetFloat("saveX", 0);
            animator[1].SetFloat("saveY", -1);
        }
        else
        {
            animator[1].SetFloat("saveX", animator[0].GetFloat("saveX"));
            animator[1].SetFloat("saveY", animator[0].GetFloat("saveY"));
        }

        switch (equip)
        {
            case 0:
                animator[0].SetTrigger("usePickUp");
                //animator[1].SetTrigger("usePickUp");
                break;            
            case 1:
                animator[0].SetTrigger("useHoe");
                animator[1].SetTrigger("useHoe");
                break;            
            case 2:
                animator[0].SetTrigger("useWater");
                animator[1].SetTrigger("useWater");
                break;
        }
    }

    public void DeathAnimation(bool value)
    {
        animator[0].SetBool("isDeath", value);
        animator[1].SetBool("isDeath", value);
    }
}
