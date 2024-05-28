using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioSource _vocalsAudioSource;
    [SerializeField] private AudioClip[] _sxfAudioClip;
    [SerializeField] private AudioClip[] _vocalsAudioClip;

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    public void PlaySFX(AudioSource audioSource, AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            Debug.LogError("AudioSource or AudioClip is null!");
            return;
        }

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void SFX(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex > _sxfAudioClip.Length)
        {
            Debug.LogError("Invalid Clip Index!");
            return;
        }
        _sfxAudioSource.clip = _sxfAudioClip[clipIndex];
        _sfxAudioSource.Play();
    }

    public void Vocals(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex > _vocalsAudioClip.Length)
        {
            Debug.LogError("Invalid Clip Index!");
            return;
        }
        _vocalsAudioSource.clip = _vocalsAudioClip[clipIndex];
        _vocalsAudioSource.Play();
    }
}
