using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip[] _audioClip;

    public override void Init()
    {
        base.Init(); //Turns this class into a singleton
    }

    public void PlaySFX(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex > _audioClip.Length)
        {
            Debug.LogError("Invalid clip index!" + " Clip Index: " + clipIndex);
            return;
        }

        _sfxAudioSource.clip = _audioClip[clipIndex];
        _sfxAudioSource.Play();
    }

    public void StopAudio()
    {
        _sfxAudioSource.Stop();
    }
}
