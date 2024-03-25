using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterEventController _controller;

    private Rigidbody2D _rigidbody2D;
    private Vector2 _movementDirection = Vector2.zero;
    private Vector2 _saveDirection = Vector2.zero;

    [SerializeField] private float speed;


    private void Awake()
    {
        _controller = GetComponent<CharacterEventController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _controller.OnMoveEvent += Move;        
    }

    private void FixedUpdate()//1초에 50번 일정하게부름 델타타임이 피료가음슴
    {
        if (GameManager.Instance.sceneChangeManager.isMapChange == true)
            ApplyMovement(_saveDirection);
        else
            ApplyMovement(_movementDirection);
    }

    private void Move(Vector2 dirction)
    {
        _movementDirection = dirction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        speed = PlayerStatus.instance.playerSpeed;

        direction *= speed;

        _rigidbody2D.velocity = direction;
        //velocity = 방향, 속도 //direction방향으로 5라는 만큼 이동
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bed")
            GameManager.Instance.uIManager.UIOn(GameManager.Instance.uIManager.sleepInfoUI);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameManager.Instance.uIManager.UIOff(GameManager.Instance.uIManager.sleepInfoUI);
    }
}