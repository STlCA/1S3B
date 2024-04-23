using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

public class SoundManager : Manager
{
    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.2f; // BGM 볼륨
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 0.5f; // SFX (효과음) 볼륨

    private AudioSource musicAudioSource; // 배경음악용 AudioSource
    private AudioSource sfxAudioSource; // 효과음용 AudioSource

    public List<AudioClip> musicClips; // BGM 클립 리스트
    public List<AudioClip> sfxClips; // SFX 클립 리스트

    private void Awake()
    {
        // BGM용 AudioSource 설정
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        musicAudioSource.volume = musicVolume;
        musicAudioSource.loop = true;

        // SFX용 AudioSource 설정 ( loop 안함 )
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.volume = sfxVolume; // SFX 볼륨 설정
    }

    // 낮 음악이 재생되고 있는지
    public bool IsPlayingDayMusic()
    {
        return musicAudioSource.clip == musicClips[0] && musicAudioSource.isPlaying;
    }

    // 밤 음악이 재생되고 있는지
    public bool IsPlayingNightMusic()
    {
        return musicAudioSource.clip == musicClips[1] && musicAudioSource.isPlaying;
    }

    public void PlayMusic(int index)
    {
        musicAudioSource.clip = musicClips[index];
        musicAudioSource.Play();
    }

    public void PlaySFX(SFXSound sfx)
    {
        sfxAudioSource.PlayOneShot(sfxClips[(int)sfx], sfxVolume);
    }

    private void Start()
    {
        PlayMusic(0); // 낮 BGM 재생
    }

}
