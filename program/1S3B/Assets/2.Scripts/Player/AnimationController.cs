using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AnimationController : AnimationBase
{
    void Start()
    {
        controller.OnMoveEvent += MoveAnimation;
    }

    private void Update()
    {
        //if (GameManager.Instance.sceneChangeManager.isMapChange == true)
        //    animator.speed = 0;
        //else
        //    animator.speed = 1;
    }

    public void MoveAnimation(Vector2 direction)
    {  
        animator.SetBool("isWalking", direction.magnitude > 0f);

        if (direction.magnitude <= 0f)
        {
            animator.SetFloat("saveX", animator.GetFloat("inputX"));
            animator.SetFloat("saveY", animator.GetFloat("inputY"));
        }

        animator.SetFloat("inputX", direction.x);
        animator.SetFloat("inputY", direction.y);
    }


}
