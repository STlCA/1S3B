using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEventController : MonoBehaviour
{
    public delegate void OnMoveDel(Vector2 direction);
    public event OnMoveDel OnMoveEvent;

    public delegate void OnClickDel(PlayerEquipmentType equipmentType, Vector2 pos);
    public event OnClickDel OnClickEvent;

    public void CallMoveEvent(Vector2 direction)
    {
        OnMoveEvent?.Invoke(direction);
    }

    public void CallClickEvent(PlayerEquipmentType equipmentType, Vector2 pos)
    {
        OnClickEvent?.Invoke(equipmentType,pos);
    }
}
