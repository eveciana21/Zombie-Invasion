using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoSingleton<AudioManager>
{
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

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopAudio(AudioSource audioSource)
    {
        audioSource.Stop();
    }
}
