using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Objects/Weapons/Weapons Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    [Header("Color over LifeTime")]
    public Gradient colorGradient;

    [Header("WEAPON SHOOT EFFECT DETAILS")]
    public float duration = 0.5f;

    public float startParticaleSize = 3f;
    public float startParticaleSpeed = 3f;

    public float startLifeTime = 0.5f;

    public int maxParticleNumber = 100;

    public float effectGravity = -0.01f;

    [Header("Emission")]
    public int emissionRate = 100;
    public int burstParticleNumber = 20;


    [Header("Texture Sheet Animaton")]
    public Sprite sprite;

    [Header("Velocity over lifeTime")]
    public Vector3 velocityOverLifeTimeMin;
    public Vector3 velocityOverLifeTimeMax;
    [Header("Prefab")]
    public GameObject weaponShootEffectPrefab;
}
