using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    [Header("Drop")]
    public Transform sprite;
    public Transform floor;

    [Header("Looting")]
    public CircleCollider2D circleCollider;

    //Drop
    private int maxBounce = 3;
    private float xForce = 5;
    private float yForce = 10;
    private float gravity = 25;

    private Vector2 direction;
    private int currentBounce = 0;
    private bool isGrounded = true;

    private float maxHeight;
    private float currentHeight;

    //Loot
    private float range = 2;
    private Vector3 startPos;
    private float currentTime;
    private float distanceSpeed = 5f;
    private bool isLooting = false;


    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;

        circleCollider.radius = range;

        currentHeight = Random.Range(yForce - 1, yForce);
        maxHeight = currentHeight;
        Init(new Vector2(Random.Range(-xForce, xForce), Random.Range(-xForce, xForce)));
    }

    public void RadiusSetting(float val)
    {
        range = val;
        circleCollider.radius = range;
    }

    private void Update()
    {
        if ((isLooting == true && currentBounce<=2)||(isLooting == false&& isGrounded == false))
        {
            currentHeight += -gravity * Time.deltaTime;
            sprite.position += new Vector3(0, currentHeight, 0) * Time.deltaTime;
            transform.position += (Vector3)direction * Time.deltaTime;

            float totalVelocity = Mathf.Abs(currentHeight) + Mathf.Abs(maxHeight);
            float scaleXY = Mathf.Abs(currentHeight) / totalVelocity;
            floor.localScale = Vector2.one * Mathf.Clamp(scaleXY, 0.5f, 1.0f);

            CheckFloorHit();
        }

        if (isLooting == true && currentBounce>2  )
            LootingItem();
    }

    private void Init(Vector2 direction)
    {
        isGrounded = false;
        maxHeight /= 1.5f;
        this.direction = direction;
        currentHeight = maxHeight;
        currentBounce++;

        if (currentBounce > 2)
            startPos = transform.position;
    }

    private void CheckFloorHit()
    {
        if (sprite.position.y < floor.position.y)
        {
            sprite.position = floor.position;
            floor.localScale = Vector3.one;

            if (currentBounce < maxBounce)
                Init(direction / 1.5f);
            else
                isGrounded = true;
        }
    }

    private void LootingItem()
    {
        Vector3 playerPos = player.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        currentTime += distanceSpeed * Time.deltaTime;
        this.transform.position = Vector3.Lerp(startPos, playerPos, currentTime);

        if (distance < 0.5f)
        {
            Destroy(gameObject);
            isLooting = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLooting == false && other.CompareTag("Player"))
        {
            isLooting = true;
            currentTime = 0;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (startPos == Vector3.zero && other.CompareTag("Player"))
        {
            isLooting = false;
            currentTime = 0;
        }
    }

}
