using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLayerSetting : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;

    private SpriteRenderer objectSR;
    private CircleCollider2D circleCollider;

    private float time;
    private float fadeTime = 1f;

    private float radius = 2;

    private Vector3 playerPos;
    private float lootSpeed = 7f;
    private float distanceSpeed = 5f;
    private bool isLooting = false;


    private void Start()
    {
        gameManager = GameManager.Instance;
        player = gameManager.Player;

        objectSR = GetComponent<SpriteRenderer>();
        objectSR.sortingOrder = (int)(transform.position.y * 100 * -1);

        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = radius;
    }

    private void Update()
    {
        if (isLooting == true)
            LootingItem();
        
    }

    public void RadiusSetting(float val)
    {
        radius = val;
    }

    private void LootingItem()
    {
        playerPos = player.transform.position;
        Vector3 direction = (playerPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, playerPos);

        float speed = lootSpeed * distanceSpeed / distance;
        if (speed < distanceSpeed)
            speed = distanceSpeed;

        transform.position += direction * speed * Time.deltaTime;

        if (distance < 0.1f)
        {
            isLooting = false;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&CompareTag("Tree"))
        {
            if (ObjectSortingOrderCheck(other) == true)//other���� �� ������
            {
                StopCoroutine("PlusAlpha");
                MinusAlphaStart();
            }
        }

        if (other.CompareTag("Player") && CompareTag("DropItem"))
            isLooting = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&CompareTag("Tree"))
        {
            StopCoroutine("MinusAlpha");
            PlusAlphaStart();
        }
    }

    public void MinusAlphaStart()
    {
        StartCoroutine("MinusAlpha");
    }

    public void PlusAlphaStart()
    {
        StartCoroutine("PlusAlpha");
    }

    public IEnumerator MinusAlpha()
    {
        Color alpha = objectSR.color;
        time = 0f;

        while (alpha.a > 0.5f)
        {
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(alpha.a, 0.5f, time);
            objectSR.color = alpha;
            yield return null;
        }
    }

    public IEnumerator PlusAlpha()
    {
        Color alpha = objectSR.color;
        time = 0f;

        while (alpha.a < 1f)
        {
            time += Time.deltaTime / fadeTime;
            alpha.a = Mathf.Lerp(alpha.a, 1, time);
            objectSR.color = alpha;
            yield return null;
        }
    }

    private bool ObjectSortingOrderCheck(Collider2D other)
    {
        SpriteRenderer[] otherSR = other.GetComponentsInChildren<SpriteRenderer>();

        foreach(SpriteRenderer sr in otherSR)
        {
            if (sr.sortingOrder > objectSR.sortingOrder)
                return false;
        }

        return true;
    }
}
