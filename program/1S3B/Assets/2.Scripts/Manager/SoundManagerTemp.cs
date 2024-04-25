using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

public class SoundManagerTemp : Manager
{
    /*    [SerializeField][Range(0f, 1f)] private float musicVolume = 0.2f; // BGM 볼륨
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
            //sfxAudioSource.PlayOneShot(sfxClips[(int)sfx], sfxVolume);
        }

        private void Start()
        {
            PlayMusic(0); // 낮 BGM 재생
        }*/

    [Header("AudioSource")]
    public AudioSource BGMSource;
    public AudioSource EffectSource;
    public AudioSource GameEffectSource;
    /*
     using Constants;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class SoundManager : Manager
    {
        [Header("AudioSource")]
        public AudioSource BGMSource;
        public AudioSource EffectSource;
        public AudioSource GameEffectSource;

        [Header("VolumeSlider")]
        public Slider BGMSlider;
        public Slider EffectSlider;
        public Slider GameEffectSlider;

        [Header("AudioClipList")]
        public List<AudioClip> AudioClipList;

        private float bgmVolume = 1;
        private float effectVolume = 1;
        private float gameEffectVolume = 1;

        public void BGMSoundSlider(Image image)
        {
            if (BGMSource.volume == 0)
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
            GameEffectSource.PlayOneShot(AudioClipList[index], 1);
        }
    }
    */
}
