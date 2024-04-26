using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PopUpController : Manager 
{    
    public PlayerInput _playerInput;

    //ON OFF Change
    public void UIOnOff(GameObject ui)
    {
        if (ui == null)
            return;

        if (ui.activeSelf == true)
            ui.SetActive(false);
        else
            ui.SetActive(true);
    }

    //UION / OFF
    public void UIOn(GameObject ui)
    {
        if (ui == null)
            return;
        ui.SetActive(true);

        SwitchPlayerInputAction(true);
    }
    public void UIOff(GameObject ui)
    {
        if (ui == null)
            return;

        ui.SetActive(false);
        Time.timeScale = 1.0f;

        SwitchPlayerInputAction(false);
    }
    public void UIOffStopTime(GameObject ui)
    {
        if (ui == null)
            return;

        ui.SetActive(false);
    }

    // UI가 켜졌을 때 플레이어의 움직임을 막기 위한 플레이어 InputAction
    public void SwitchPlayerInputAction(bool isUI)
    {
        if (isUI)
        {
            // UI가 활성화 되면 플레이어의 움직임을 제한
            _playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("PlayerAction");
        }
    }
}
