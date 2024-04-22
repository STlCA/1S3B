using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSoundManager : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource BGMSource;
    public AudioSource EffectSource;

    [Header("AudioClipList")]
    public List<AudioClip> AudioClipList;

    [HideInInspector] public bool effectSoundVolume = true;

    public void BGMSoundOnOff()
    {
        if (BGMSource.volume == 0)
            BGMSource.volume = 1;
        else
            BGMSource.volume = 0;
    }

    public void EffectSoundOnOff()
    {
        if (effectSoundVolume == true)
            effectSoundVolume = false;
        else
            effectSoundVolume = true;
    }

    public void AudioClipPlay(int index)
    {
        EffectSource.PlayOneShot(AudioClipList[index], 1);
    }
}
