using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler
{
    public StartSoundManager startSoundManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        startSoundManager.AudioClipPlay((int)StartSceneAudioClip.MouseOver1);
    }
}
