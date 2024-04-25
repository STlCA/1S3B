using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;
    private CharacterEventController _controller;
    private SceneChangeManager sceneChangeManager;
    private UIManager uiManager;
    private PopUpController popUpController;

    public SortingGroup playerSprite;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _movementDirection = Vector2.zero;
    private Vector2 _zeroDirection = Vector2.zero;

    // UI가 켜졌을 때 플레이어의 움직임을 막기 위한 플레이어 InputAction
    private PlayerInput _playerInput;

    private bool isChange = false;

    [SerializeField] private float speed;

    private void Awake()
    {
        _controller = GetComponent<CharacterEventController>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;
        sceneChangeManager = gameManager.SceneChangeManager;
        uiManager = gameManager.UIManager;
        popUpController = gameManager.PopUpController;

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

    private void Move(Vector2 dirction, bool isUse = false, bool isCarry = false)
    {
        _movementDirection = dirction;
    }

    private void ApplyMovement(Vector2 direction)
    {
        speed = player.playerSpeed;

        direction *= speed;

        _rigidbody2D.velocity = direction;
        //velocity = 방향, 속도 //direction방향으로 5라는 만큼 이동

        playerSprite.sortingOrder = (int)(transform.position.y * 1000 * -1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bed")
        {
            popUpController.UIOn(uiManager.sleepInfoUI);
            SwitchPlayerInputAction(true);
        }

        if(other.tag == "Counter")
        {
            uiManager.shopUI.shop.OpenShop();
        }
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