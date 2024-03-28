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


    private void Start()
    {
        objectSR = GetComponent<SpriteRenderer>();
        objectSR.sortingOrder = (int)(transform.position.y * 100 * -1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (ObjectSortingOrderCheck(other) == true)//other들이 다 작으면
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
