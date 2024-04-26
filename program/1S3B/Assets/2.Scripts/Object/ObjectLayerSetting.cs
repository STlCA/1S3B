using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ObjectLayerSetting : MonoBehaviour
{
    private SpriteRenderer objectSR = null;
    private SortingGroup sortingGroup = null;

    private float time;
    private float fadeTime = 1f;

    public NeedUpdateObject type = NeedUpdateObject.None;

    private void Start()
    {
        if (type == NeedUpdateObject.NPC)
        {
            sortingGroup = GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = (int)(transform.position.y * 1000 * -1);
        }
        else
        {
            objectSR = GetComponent<SpriteRenderer>();
            objectSR.sortingOrder = (int)(transform.position.y * 1000 * -1);
        }
    }

    private void Update()
    {
        if (type == NeedUpdateObject.Need)
            objectSR.sortingOrder = (int)(transform.position.y * 1000 * -1);
        else if (type == NeedUpdateObject.NPC)
            sortingGroup.sortingOrder = (int)(transform.position.y * 1000 * -1);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (type != NeedUpdateObject.NPC && other.CompareTag("Player"))
        {
            StopCoroutine("PlusAlpha");
            MinusAlphaStart();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (type != NeedUpdateObject.NPC && other.CompareTag("Player"))
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
