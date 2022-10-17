using System.Collections;
using UnityEngine;
using System;
using Puppy.Engine.Utilities;

[System.Serializable]
public class Audio
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField] private Type type = Type.Sound;

    public float Length => audioClip != null ? audioClip.length : 0f;

    public void Play()
    {
        switch (type)
        {
            case Type.Music:
                AudioManager.Instance.PlayMusic(audioClip, volume);
                break;
            default:
                AudioManager.Instance.PlaySound(audioClip, volume);
                break;
        }
    }

    public enum Type
    {
        Sound,
        Music,
    }
}

public class AudioManager : Singleton<AudioManager>
{
    public const string MusicEnabledKey = "MusicEnabled";
    public const string SoundEnabledKey = "SoundEnabled";
    public const string VibrateEnabledKey = "VibrateEnabled";

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource soundAudioSource;

    private Coroutine fadeCoroutine;

    public bool MusicEnabled
    {
        get { return GamePrefData.GetBool(MusicEnabledKey, true) && MusicVolume > 0f; }
        set
        {
            bool current = GamePrefData.GetBool(MusicEnabledKey, true);
            if (current == value) return;
            GamePrefData.SetBool(MusicEnabledKey, value);
            Debug.Log($"[SoundManager] Music Enabled: {value}");

            if (value) PlayMusic(musicAudioSource.clip);
            else StopMusic();
        }
    }

    public bool SoundEnabled
    {
        get { return GamePrefData.GetBool(SoundEnabledKey, true) && SoundVolume > 0f; }
        set { GamePrefData.SetBool(SoundEnabledKey, value); }
    }

    public bool VibrateEnabled
    {
        get { return GamePrefData.GetBool(VibrateEnabledKey, true); }
        set { GamePrefData.SetBool(VibrateEnabledKey, value); }
    }

    public float MusicVolume
    {
        get { return GamePrefData.GetFloat("MusicVolume", 1f); }
        set
        {
            float v = Mathf.Clamp01(value);
            GamePrefData.SetFloat("MusicVolume", v);
            if (musicAudioSource) musicAudioSource.volume = v;
        }
    }

    public float SoundVolume
    {
        get { return GamePrefData.GetFloat("SoundVolume", 1f); }
        set
        {
            float v = Mathf.Clamp01(value);
            GamePrefData.SetFloat("SoundVolume", v);
            if (soundAudioSource) soundAudioSource.volume = v;
        }
    }

    #region Music
    public void PlayMusic(AudioClip audioClip = null, float volumeScale = 1f, bool loop = true, float fadeDuration = 1f)
    {
        if (musicAudioSource == null || audioClip == null || !MusicEnabled)
        {
            if (musicAudioSource && audioClip) musicAudioSource.clip = audioClip;
            return;
        }

        bool isFade = fadeDuration > 0f;

        if (isFade)
        {
            Fade(musicAudioSource, MusicVolume, 0f, fadeDuration, () => {
                musicAudioSource.clip = audioClip;
                musicAudioSource.loop = loop;
                musicAudioSource.Play();

                Fade(musicAudioSource, 0f, MusicVolume * volumeScale);
            });
        }
        else
        {
            musicAudioSource.clip = audioClip;
            musicAudioSource.loop = loop;
            musicAudioSource.volume = MusicVolume * volumeScale;
        }
    }

    public void StopMusic(float fadeDuration = 1f, Action onComplete = null)
    {
        if (musicAudioSource == null)
        {
            onComplete?.Invoke();
            return;
        }

        bool isFade = fadeDuration > 0f;

        if (isFade)
        {
            Fade(musicAudioSource, 1f, 0f, fadeDuration, () => {
                musicAudioSource.Stop();
                onComplete?.Invoke();
            });
        }
        else
        {
            musicAudioSource.Stop();
            onComplete?.Invoke();
        }
    }
    #endregion

    #region Sound    
    public void PlaySound(AudioClip audioClip, float volumeScale = 1f)
    {
        if (soundAudioSource == null || audioClip == null || !SoundEnabled)
            return;
        soundAudioSource.PlayOneShot(audioClip, volumeScale);
    }
    public void StopSound()
    {
        if (soundAudioSource == null)
            return;
        soundAudioSource.Stop();
    }
    #endregion

    #region Vibrate
    public void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (VibrateEnabled) Handheld.Vibrate();
#endif
    }
    #endregion

    #region Helper
    private void Fade(AudioSource audio, float from, float to, float duration = 1, Action onCompleted = null)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        StartCoroutine(IEFadeAudioSound(audio, from, to, duration, onCompleted));
    }

    private IEnumerator IEFadeAudioSound(AudioSource audioSource, float from, float to, float duration = 1, Action onCompleted = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        audioSource.volume = to;
        if (onCompleted != null) onCompleted.Invoke();
    }
    #endregion
}