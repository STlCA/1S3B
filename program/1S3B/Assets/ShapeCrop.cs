using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCrop : MonoBehaviour
{
    public Animator myAnimator;
    private bool isAnim = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAnim == false && other.CompareTag("Player") == true)
        {
            isAnim = true;

            Vector3 direction = other.transform.position - transform.position;

            myAnimator.SetTrigger("isTrigger");
            myAnimator.SetFloat("inputX", direction.x);
            myAnimator.SetFloat("inputY", direction.y);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isAnim == true && other.CompareTag("Player") == true)
            isAnim = false;
    }
}
