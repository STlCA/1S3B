using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEventController : MonoBehaviour
{
    public delegate void OnMoveDel(Vector2 directionm,bool isUse,bool isCarry);
    public event OnMoveDel OnMoveEvent;

    public delegate void OnClickDel(PlayerEquipmentType equipmentType, Vector2 pos);
    public event OnClickDel OnClickEvent;

    public void CallMoveEvent(Vector2 direction, bool isUse = false, bool isCarry = false)
    {
        OnMoveEvent?.Invoke(direction, isUse, isCarry);
    }

    public void CallClickEvent(PlayerEquipmentType equipmentType, Vector2 pos)
    {
        OnClickEvent?.Invoke(equipmentType,pos);
    }
}
