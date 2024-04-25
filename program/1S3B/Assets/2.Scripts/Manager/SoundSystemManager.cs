using Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSystemManager : Manager
{
    [Header("AudioSource")]
    public AudioSource BGMSource;
    public AudioSource WeatherSource;
    public AudioSource EffectSource;
    public AudioSource GameEffectSource;

    [Header("VolumeSlider")]
    public Slider BGMSlider;
    public Slider WeatherSlider;
    public Slider EffectSlider;
    public Slider GameEffectSlider;

    [Header("AudioClipList")]
    public List<AudioClip> audioClipList;

    [Header("PlayerAudioClip")]
    public List<AudioClip> playerAudioList;

    private float bgmVolume = 1;
    private float weatherVolume = 1;
    private float effectVolume = 1;
    private float gameEffectVolume = 1;

    public void BGMSoundSlider(Image image)
    {
        if (BGMSource.volume == 0)
            image.color = Color.gray;
        else
            image.color = Color.white;
    }
    public void WeatherSoundSlider(Image image)
    {
        if (WeatherSource.volume == 0)
            image.color = Color.gray;
        else
            image.color = Color.white;
    }
    public void EffectSoundSlider(Image image)
    {
        if (EffectSource.volume == 0)
            image.color = Color.gray;
        else
            image.color = Color.white;
    }
    public void GameEffectSoundSlider(Image image)
    {
        if (GameEffectSource.volume == 0)
            image.color = Color.gray;
        else
            image.color = Color.white;
    }
    public void BGMSoundOnOff(Image image)
    {
        if (BGMSource.volume == 0)
        {
            image.color = Color.white;
            BGMSource.volume = bgmVolume;
            BGMSlider.value = bgmVolume;
        }
        else
        {
            image.color = Color.gray;
            bgmVolume = BGMSource.volume;
            BGMSource.volume = 0;
            BGMSlider.value = 0;
        }
    }

    public void WeatherSoundOnOff(Image image)
    {
        if (WeatherSource.volume == 0)
        {
            image.color = Color.white;
            WeatherSource.volume = weatherVolume;
            WeatherSlider.value = weatherVolume;
        }
        else
        {
            image.color = Color.gray;
            weatherVolume = WeatherSource.volume;
            WeatherSource.volume = 0;
            WeatherSlider.value = 0;
        }
    }

    public void EffectSoundOnOff(Image image)
    {
        if (EffectSource.volume == 0)
        {
            image.color = Color.white;
            EffectSource.volume = effectVolume;
            EffectSlider.value = effectVolume;
        }
        else
        {
            image.color = Color.gray;
            effectVolume = EffectSource.volume;
            EffectSource.volume = 0;
            EffectSlider.value = 0;
        }
    }

    public void GameEffectSoundOnOff(Image image)
    {
        if (GameEffectSource.volume == 0)
        {
            image.color = Color.white;
            GameEffectSource.volume = gameEffectVolume;
            GameEffectSlider.value = gameEffectVolume;
        }
        else
        {
            image.color = Color.gray;
            gameEffectVolume = GameEffectSource.volume;
            GameEffectSource.volume = 0;
            GameEffectSlider.value = 0;
        }
    }

    public void GameAudioClipPlay(int index)
    {
        GameEffectSource.PlayOneShot(audioClipList[index], 1);
    }
    public void PlayerAudioClipPlay(int index)
    {
        EffectSource.PlayOneShot(playerAudioList[index], 1);
    }

    public void WalkSoundChange(bool isRun)
    {
        if (isRun)
        {
            EffectSource.clip = playerAudioList[(int)PlayerAudioClip.FarmRun];
        }
        else
            EffectSource.clip = playerAudioList[(int)PlayerAudioClip.FarmWalk];
    }
}
