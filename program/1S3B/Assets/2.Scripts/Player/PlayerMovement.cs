using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;

    private Rigidbody2D _rigidbody2D;
    private Vector2 _movementDirection = Vector2.zero;

    public float speed = 5f;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _controller.OnMoveEvent += Move;
    }

    private void FixedUpdate()
    {
        ApplyMovement(_movementDirection);
    }

    private void Move(Vector2 dirction)
    {
        _movementDirection = dirction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        direction *= speed;

        _rigidbody2D.velocity = direction; 
        //velocity = 방향, 속도 //direction방향으로 5라는 만큼 이동
    }
}