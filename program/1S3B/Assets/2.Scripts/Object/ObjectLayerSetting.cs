using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectLayerSetting : MonoBehaviour
{
    private SpriteRenderer objectSR;

    private float time;
    private float fadeTime = 1f;

    public NeedUpdateObject type = NeedUpdateObject.None;

    private void Start()
    {
        objectSR = GetComponent<SpriteRenderer>();        
        objectSR.sortingOrder = (int)(transform.position.y * 1000 * -1);
    }

    private void Update()
    {
        if (type == NeedUpdateObject.Need)
            objectSR.sortingOrder = (int)(transform.position.y * 1000 * -1);
    }

    private bool ObjectSortingOrderCheck(Collider2D other)
    {
        SpriteRenderer[] otherSR = other.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in otherSR)
        {
            if (sr.sortingOrder > objectSR.sortingOrder)
                return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ObjectSortingOrderCheck(other) == true)//other == player
            {
                StopCoroutine("PlusAlpha");
                MinusAlphaStart();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
}
