using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;
    private CharacterEventController _controller;
    private SceneChangeManager sceneChangeManager;
    private UIManager uiManager;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer[] _spriteRenderer;
    private Vector2 _movementDirection = Vector2.zero;
    private Vector2 _zeroDirection = Vector2.zero;

    private bool isChange = false;

    [SerializeField] private float speed;

    private void Awake()
    {
        _controller = GetComponent<CharacterEventController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;
        sceneChangeManager = gameManager.SceneChangeManager;
        uiManager = gameManager.UIManager;

        _controller.OnMoveEvent += Move;
        sceneChangeManager.mapChangeAction += ChangeDirection;
    }

    private void FixedUpdate()//1초에 50번 일정하게부름 델타타임이 피료가음슴
    {
        if (isChange == true)
            ApplyMovement(_zeroDirection);
        else
            ApplyMovement(_movementDirection);
    }

    private void ChangeDirection(bool isChange)
    {
        this.isChange = isChange;
    }

    private void Move(Vector2 dirction)
    {
        _movementDirection = dirction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        speed = player.playerSpeed;

        direction *= speed;

        _rigidbody2D.velocity = direction;
        //velocity = 방향, 속도 //direction방향으로 5라는 만큼 이동

        for (int i = 0; i < _spriteRenderer.Length; i++)
        {
            _spriteRenderer[i].sortingOrder = (int)(transform.position.y * 100 * -1) + i;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bed")
        {
            uiManager.UIOn(uiManager.sleepInfoUI);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        uiManager.UIOff(uiManager.sleepInfoUI);
    }
}