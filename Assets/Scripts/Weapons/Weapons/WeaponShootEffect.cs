using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem shootEffectParticleSystem;

    private void Awake()
    {
        shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    public void SetShootEffect(WeaponShootEffectSO shootEffect,float aimAngle)
    {
        SetShootEffectColorGradient(shootEffect.colorGradient);

        SetShootEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticaleSize, shootEffect.startParticaleSpeed,
            shootEffect.startLifeTime, shootEffect.effectGravity, shootEffect.maxParticleNumber);

        SetShootEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);

        SetShootEffectRotation(aimAngle);

        SetShootEffectParticleSprite(shootEffect.sprite);

        SetShootEffectVelocityOverLifeTime(shootEffect.velocityOverLifeTimeMin, shootEffect.velocityOverLifeTimeMax);
    }



    private void SetShootEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetShootEffectParticleStartingValues(float duration, float startParticaleSize, float startParticaleSpeed, float startLifeTime, float effectGravity, int maxParticleNumber)
    {
        ParticleSystem.MainModule mainModule = shootEffectParticleSystem.main;
        mainModule.duration = duration;
        mainModule.startSize = startParticaleSize;
        mainModule.startSpeed = startParticaleSpeed;
        mainModule.startLifetime = startLifeTime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetShootEffectParticleEmission(int emissionRate, int burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = shootEffectParticleSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }

    private void SetShootEffectRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = shootEffectParticleSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetShootEffectVelocityOverLifeTime(Vector3 velocityOverLifeTimeMin, Vector3 velocityOverLifeTimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = shootEffectParticleSystem.velocityOverLifetime;

        ParticleSystem.MinMaxCurve minMaxCurveX = new ParticleSystem.MinMaxCurve();
        minMaxCurveX.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveX.constantMin = velocityOverLifeTimeMin.x;
        minMaxCurveX.constantMax = velocityOverLifeTimeMax.x;

        ParticleSystem.MinMaxCurve minMaxCurveY = new ParticleSystem.MinMaxCurve();
        minMaxCurveY.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveY.constantMin = velocityOverLifeTimeMin.y;
        minMaxCurveY.constantMax = velocityOverLifeTimeMax.y;

        ParticleSystem.MinMaxCurve minMaxCurveZ = new ParticleSystem.MinMaxCurve();
        minMaxCurveZ.mode = ParticleSystemCurveMode.TwoConstants;
        minMaxCurveZ.constantMin = velocityOverLifeTimeMin.z;
        minMaxCurveZ.constantMax = velocityOverLifeTimeMax.z;


    }
}
