using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSoundManager : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource BGMSource;
    public AudioSource EffectSource;
    public AudioSource GameEffectSource;

    [Header("AudioClipList")]
    public List<AudioClip> AudioClipList;

    private float bgmVolume = 1;
    private float effectVolume = 1;
    private float gameEffectVolume = 1;

    public void BGMSoundOnOff(Image image)
    {
        if (BGMSource.volume == 0)
        {
            image.color = Color.white;
            BGMSource.volume = bgmVolume;
        }
        else
        {
            image.color = Color.gray;
            bgmVolume = BGMSource.volume;
            BGMSource.volume = 0;
        }
    }

    public void EffectSoundOnOff(Image image)
    {
        if (EffectSource.volume == 0)
        {
            image.color = Color.white;
            EffectSource.volume = effectVolume;
        }
        else
        {
            image.color = Color.gray;
            effectVolume = EffectSource.volume;
            EffectSource.volume = 0;
        }
    }

    public void GameEffectSoundOnOff(Image image)
    {
        if (GameEffectSource.volume == 0)
        {
            image.color = Color.white;
            GameEffectSource.volume = gameEffectVolume;
        }
        else
        {
            image.color = Color.gray;
            gameEffectVolume = GameEffectSource.volume;
            GameEffectSource.volume = 0;
        }
    }

    public void AudioClipPlay(int index)
    {
        GameEffectSource.PlayOneShot(AudioClipList[index], 1);
    }
}
