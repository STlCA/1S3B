using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AnimationController : Animation
{
    void Start()
    {
        //controller.OnMoveEvent += Animation;
    }

    public void Animation(Vector2 direction)
    {
        //if (direction.x > 0f)
        //{
        //    animator.SetBool("isRight", true);            
        //    animator.SetBool("isLeft", false);
        //    animator.SetBool("isUp", false);
        //    animator.SetBool("isDown", false);
        //    animator.SetBool("isWalking", true);
        //}
        //else if (direction.x < 0f)
        //{
        //    animator.SetBool("isRight", false);
        //    animator.SetBool("isLeft", true);
        //    animator.SetBool("isUp", false);
        //    animator.SetBool("isDown", false);
        //    animator.SetBool("isWalking", true);
        //}
        //else if (direction.y > 0f)
        //{
        //    animator.SetBool("isRight", false);
        //    animator.SetBool("isLeft", false);
        //    animator.SetBool("isUp", true);            
        //    animator.SetBool("isDown", false);
        //    animator.SetBool("isWalking", true);
        //}
        //else if (direction.y < 0f && direction.magnitude > 0f)
        //{
        //    animator.SetBool("isRight", false);
        //    animator.SetBool("isLeft", false);
        //    animator.SetBool("isUp", false);
        //    animator.SetBool("isDown", true);
        //    animator.SetBool("isWalking", true);
        //}
        //else
        //{
        //    animator.SetBool("isWalking", false);
        //}

        

        ////animator.SetBool("isWalking", direction.magnitude > 0f);
        ////    animator.SetFloat("xValue", direction.x);
        ////    animator.SetFloat("yValue", direction.y);
        //
       //if (animator.GetFloat("xValue") != direction.x)
       //{
       //    animator.SetBool("isWalking", true);
       //    animator.SetFloat("xValue", direction.x);
       //}
       //else if (animator.GetFloat("yValue") != direction.y)
       //{
       //    animator.SetBool("isWalking", true);
       //    animator.SetFloat("yValue", direction.y);
       //}
       //else
       //   animator.SetBool("isWalking", false);
        //
        //if (animator.GetFloat("xValue") == direction.x && animator.GetFloat("yValue") == direction.y && animator.GetBool("isWalking") == true)
        //    animator.SetBool("isWalking", false);
    }
}
