using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEventController : MonoBehaviour
{
    public delegate void OnMoveDel(Vector2 direction);
    public event OnMoveDel OnMoveEvent;

    public void CallMoveEvent(Vector2 direction)
    {
        OnMoveEvent?.Invoke(direction);
    }
}
