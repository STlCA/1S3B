using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBase : MonoBehaviour
{
    protected Animator animator;
    protected CharacterEventController controller;

    protected void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterEventController>();
    }
}
