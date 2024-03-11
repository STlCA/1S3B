using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : CharacterController
{
    private Camera mainCamera;

    public GameObject targetObj;
    private TargetSetting targetSetting;

    private void Awake()
    {
        mainCamera = Camera.main;
        targetSetting = targetObj.GetComponent<TargetSetting>();
    }

    public void OnMove(InputValue value)
    {
        Vector2 moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);

        CallMoveEvent(moveInput);

        if(moveInput == Vector2.zero)
            targetSetting.gameObject.SetActive(true);
        else
            targetSetting.gameObject.SetActive(false);
    }
    public void OnMouse(InputValue value)
    {
        Vector2 position = value.Get<Vector2>();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(position);

        targetSetting.SetCellPosition(worldPos);
    }

    public void OnInteraction(InputValue value)
    {
        targetSetting.TileCheck();
    }
}
