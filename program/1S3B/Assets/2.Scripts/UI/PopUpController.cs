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

    // UI�� ������ �� �÷��̾��� �������� ���� ���� �÷��̾� InputAction
    public void SwitchPlayerInputAction(bool isUI)
    {
        if (isUI)
        {
            // UI�� Ȱ��ȭ �Ǹ� �÷��̾��� �������� ����
            _playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            _playerInput.SwitchCurrentActionMap("PlayerAction");
        }
    }
}
