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
        if (direction.magnitude <= 0f)
        {
            animator.SetFloat("saveX", animator.GetFloat("inputX"));
            animator.SetFloat("saveY", animator.GetFloat("inputY"));
        }

        animator.SetFloat("inputX", direction.x);
        animator.SetFloat("inputY", direction.y);

        animator.SetBool("isWalking", direction.magnitude > 0f);
    }

    public void StopAnimation(bool value)
    {
        if (value == true)
        {
            if (oneTimeSave == false)
            {
                saveDirection.x = animator.GetFloat("inputX");
                saveDirection.y = animator.GetFloat("inputY");

                oneTimeSave = true;
            }

            animator.SetBool("isWalking", false);

            animator.speed = 0;

            animator.SetFloat("saveX", saveDirection.x);
            animator.SetFloat("saveY", saveDirection.y);
        }
        else
        {
            GameManager.Instance.sceneChangeManager.isReAnim = false;
            oneTimeSave = false;

            animator.speed = 1;

            if (animator.GetFloat("inputX") != 0 || animator.GetFloat("inputY") != 0)
            {
                MoveAnimation(saveDirection);
            }
        }
    }

    public void UseAnimation()
    {

    }
}
