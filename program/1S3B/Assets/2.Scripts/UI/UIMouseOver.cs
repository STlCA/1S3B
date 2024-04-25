using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler
{
    public SoundSystemManager startSoundManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        startSoundManager.GameAudioClipPlay((int)StartSceneAudioClip.MouseOver1);
    }
}
