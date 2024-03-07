using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 300f;
    private Vector2 _movement;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_movement != null)
        {
            _rigidbody.velocity = _movement * speed * Time.deltaTime;
            Debug.Log(_movement * speed * Time.deltaTime);
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (_movement != null)
        {
            _movement = context.ReadValue<Vector2>();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                if (_movement.x == 0f || _movement.y == 0f)
                    this.transform.position += (Vector3)_movement * 1f;
                Debug.Log("탭성공");
                break;
            case InputActionPhase.Started:
                Debug.Log("눌림");
                break;
            case InputActionPhase.Canceled:
                Debug.Log("탭실패");
                break;
        }
        //슬로우탭은 0.5초이상눌러야 성공
        //탭은 0.2초이하로 눌러야 성공
        //현재 스페이스바에 탭만 걸어둠
    }
}