using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler//, IPointerExitHandler
{
    public SoundSystemManager startSoundManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        startSoundManager.GameAudioClipPlay((int)StartSceneAudioClip.MouseOver1);
    }

    /*    public void OnPointerEnter(PointerEventData eventData)
        {
            GameManager.Instance.PopUpController.SwitchPlayerInputAction(true);

            startSoundManager.GameAudioClipPlay((int)StartSceneAudioClip.MouseOver1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameManager.Instance.PopUpController.SwitchPlayerInputAction(false);
        }*/
}
