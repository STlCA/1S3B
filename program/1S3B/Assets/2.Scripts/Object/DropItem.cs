using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    private CircleCollider2D circleCollider;

    private float range = 2;

    private Vector3 startPos;
    private float currentTime;
    private float distanceSpeed = 5f;
    private bool isLooting = false;


    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;

        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = range;
    }

    public void RadiusSetting(float val)
    {
        range = val;
    }

    private void Update()
    {
        if (isLooting == true)
            LootingItem();
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
        if (other.CompareTag("Player"))
        {
            isLooting = true;
            startPos = this.transform.position;
            currentTime = 0;
        }
    }
}
