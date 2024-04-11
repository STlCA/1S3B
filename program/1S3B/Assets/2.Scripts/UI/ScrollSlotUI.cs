using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScrollSlotUI : MonoBehaviour
{
    public virtual void Init()
    {

    }
    
    public virtual void Set(int idx)
    {

    }

    public abstract void SetSlotSize(out float width, float height);
}
