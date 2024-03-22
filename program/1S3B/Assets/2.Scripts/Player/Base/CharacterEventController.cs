using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEventController : MonoBehaviour
{
    public delegate void OnMoveDel(Vector2 direction);
    public event OnMoveDel OnMoveEvent;

    public delegate void OnClickDel(int equip);
    public event OnClickDel OnClickEvent;

    public void CallMoveEvent(Vector2 direction)
    {
        OnMoveEvent?.Invoke(direction);
    }

    public void CallClickEvent(int equip)
    {
        OnClickEvent?.Invoke(equip);
    }
}
