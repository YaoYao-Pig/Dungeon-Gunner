using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Objects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    public string soundEffectName;
    public GameObject soundEffectPrefab;

    public AudioClip soundEffectClip;

    [Range(0f,1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.0f;
    [Range(0f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;

    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;
}
