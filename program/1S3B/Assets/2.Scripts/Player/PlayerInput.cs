using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : PlayerController
{
    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);

        CallMoveEvent(moveInput);
    }
}
