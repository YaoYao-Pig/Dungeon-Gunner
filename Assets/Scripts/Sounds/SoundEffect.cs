using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    public void SetSound(SoundEffectSO soundEffectSO)
    {
        audioSource.pitch = UnityEngine.Random.Range(soundEffectSO.soundEffectPitchRandomVariationMin, soundEffectSO.soundEffectPitchRandomVariationMax);
        audioSource.volume = soundEffectSO.soundEffectVolume;
        audioSource.clip = soundEffectSO.soundEffectClip;
    }
}
