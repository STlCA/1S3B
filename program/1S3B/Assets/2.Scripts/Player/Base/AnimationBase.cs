using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBase : MonoBehaviour
{
    protected Animator[] animator = new Animator[2];
    protected CharacterEventController controller;

    protected void Awake()
    {
        animator = GetComponentsInChildren<Animator>();
        controller = GetComponent<CharacterEventController>();
    }
}
