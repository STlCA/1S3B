using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCrop : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    public Animator myAnimator;
    private bool isAnim = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isAnim == false && other.CompareTag("Player") == true)
            ShapeAnimation();
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (isAnim == true && other.CompareTag("Player") == true)
            isAnim = false;
    }

    public void ShapeAnimation()
    {
        isAnim = true;

        Vector3 direction = player.transform.position - transform.position;

        myAnimator.SetTrigger("isTrigger");
        myAnimator.SetFloat("inputX", direction.x);
        myAnimator.SetFloat("inputY", direction.y);
    }
}
