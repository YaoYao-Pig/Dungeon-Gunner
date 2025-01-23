using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticalSystem;

    private void Awake()
    {
        ammoHitEffectParticalSystem = GetComponent<ParticleSystem>();
    }

    public void SetHitEffect(AmmoHitEffectSO shootEffect)
    {
        SetHitEffectColorGradient(shootEffect.colorGradient);

        SetHitEffectParticleStartingValues(shootEffect.duration, shootEffect.startParticaleSize, shootEffect.startParticaleSpeed,
            shootEffect.startLifeTime, shootEffect.effectGravity, shootEffect.maxParticleNumber);

        SetHitEffectParticleEmission(shootEffect.emissionRate, shootEffect.burstParticleNumber);


        SetHitEffectParticleSprite(shootEffect.sprite);

        SetHitEffectVelocityOverLifeTime(shootEffect.velocityOverLifeTimeMin, shootEffect.velocityOverLifeTimeMax);
    }



    private void SetHitEffectColorGradient(Gradient colorGradient)
    {
        ParticleSystem.ColorOverLifetimeModule colorOverLifetimeModule = ammoHitEffectParticalSystem.colorOverLifetime;
        colorOverLifetimeModule.color = colorGradient;
    }

    private void SetHitEffectParticleStartingValues(float duration, float startParticaleSize, float startParticaleSpeed, float startLifeTime, float effectGravity, int maxParticleNumber)
    {
        ParticleSystem.MainModule mainModule = ammoHitEffectParticalSystem.main;
        mainModule.duration = duration;
        mainModule.startSize = startParticaleSize;
        mainModule.startSpeed = startParticaleSpeed;
        mainModule.startLifetime = startLifeTime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticleNumber;
    }

    private void SetHitEffectParticleEmission(int emissionRate, int burstParticleNumber)
    {
        ParticleSystem.EmissionModule emissionModule = ammoHitEffectParticalSystem.emission;

        ParticleSystem.Burst burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        emissionModule.rateOverTime = emissionRate;
    }


    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        ParticleSystem.TextureSheetAnimationModule textureSheetAnimationModule = ammoHitEffectParticalSystem.textureSheetAnimation;
        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    private void SetHitEffectVelocityOverLifeTime(Vector3 velocityOverLifeTimeMin, Vector3 velocityOverLifeTimeMax)
    {
        ParticleSystem.VelocityOverLifetimeModule velocityOverLifetimeModule = ammoHitEffectParticalSystem.velocityOverLifetime;

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
