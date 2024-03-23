using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcAnimationData
{
    [SerializeField] private string idleParameterName = "Idel";
    [SerializeField] private string walkParameterName = "IsWalk";

    public int IdleParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }

    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
    }
}
