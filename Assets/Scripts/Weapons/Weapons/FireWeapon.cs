using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(UpdateContinueShootEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float fireRateCoolDownTimer = 0f;
    private float firePreChargTimer = 0f;

    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;
    private UpdateContinueShootEvent updateContinueShootEvent;

    private ReloadWeaponEvent reloadWeaponEvent;

    private bool hasFiredLasTime;
    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        updateContinueShootEvent = GetComponent<UpdateContinueShootEvent>();
    }
    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
        updateContinueShootEvent.OnUpdateContinueShootEvent += UpdateContinueShootEvent_OnUpdateContinueShootEvent;
    }



    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
        updateContinueShootEvent.OnUpdateContinueShootEvent += UpdateContinueShootEvent_OnUpdateContinueShootEvent;
    }

    private void FireWeaponEvent_OnFireWeapon(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponFire(fireWeaponEvent, fireWeaponEventArgs);
    }

    private void Update()
    {
        if (fireRateCoolDownTimer > 0f)
        {
            fireRateCoolDownTimer -= Time.deltaTime;
        }
    }

    private void WeaponFire(FireWeaponEvent fireWeaponEvent, FireWeaponEventArgs fireWeaponEventArgs)
    {
        WeaponPreCharge(fireWeaponEventArgs);
       
        if (fireWeaponEventArgs.fire == true)
        {
            if (IsWeaponReadyToFire())
            {
                FireAmmo(fireWeaponEventArgs.aimAngle, fireWeaponEventArgs.weaponAimAngle, fireWeaponEventArgs.weaponAimDirectionVector);
                ResetCoolDownTimer();

                ResetPreChargeTimer();
            }
        }
    }

    private void WeaponPreCharge(FireWeaponEventArgs fireWeaponEventArgs)
    {
        if (fireWeaponEventArgs.firePreviousFrame)
        {
            firePreChargTimer -= Time.deltaTime;
            if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponRechargeTime>0f&&firePreChargTimer <= 0f)
            {
                updateContinueShootEvent.CallUpdateContinueShootEvent(false);
            }
        }
        else
        {
            ResetPreChargeTimer();
        }
    }

    private void ResetPreChargeTimer()
    {
        firePreChargTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponRechargeTime;
    }
    private bool IsWeaponReadyToFire()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.canContinueFire == false && hasFiredLasTime) return false;
        if (activeWeapon.GetCurrentWeapon().weaponRemainAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteAmmo) return false;
        if (activeWeapon.GetCurrentWeapon().isWeaponReloading) return false;
        if (firePreChargTimer>0f || fireRateCoolDownTimer > 0f) return false;
        if (activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo <= 0 && !activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            reloadWeaponEvent.CallReloadWeaponEvent(activeWeapon.GetCurrentWeapon(), 0);
            return false;
        }
        return true;

    }


    private void FireAmmo(float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        AmmoDetailsSO currentAmmo = activeWeapon.GetCurrentAmmo();

        if (currentAmmo != null)
        {
            StartCoroutine(FireAmmoRoutine(currentAmmo, aimAngle, weaponAimAngle, weaponAimDirectionVector));

        }
    }

    private IEnumerator FireAmmoRoutine(AmmoDetailsSO currentAmmo,float aimAngle,float weaponAimAngle,Vector3 weaponAimDirectionVector)
    {
        int ammoCounter = 0;
        int ammoPerShoot = UnityEngine.Random.Range(currentAmmo.ammoSpawnAmountMin, currentAmmo.ammoSpawnAmountMax+1);

        float ammoSpawnInterval;
        if (ammoPerShoot > 1)
        {
            ammoSpawnInterval = UnityEngine.Random.Range(currentAmmo.ammoSpawnIntervalMin, currentAmmo.ammoSpawnIntervalMax);
        }
        else
        {
            ammoSpawnInterval = 0f;
        }

        while (ammoCounter < ammoPerShoot)
        {
            ammoCounter++;
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[UnityEngine.Random.Range(0, currentAmmo.ammoPrefabArray.Length)];
            float ammoSpeed = UnityEngine.Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);
            IFireable ammo = PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity) as IFireable;
            if (ammo == null)
            {
                Debug.LogError("Don't have IFireable in Pool Manager");
            }

            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);
            yield return new WaitForSeconds(ammoSpawnInterval);
        }


        if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
        {
            activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
            activeWeapon.GetCurrentWeapon().weaponRemainAmmo--;
        }
        weaponFiredEvent.CallWeaponFiredEvenet(activeWeapon.GetCurrentWeapon());

        WeaponSoundEffect();
    }



    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }

    private void UpdateContinueShootEvent_OnUpdateContinueShootEvent(bool target)
    {
        hasFiredLasTime = target;
    }

    private void WeaponSoundEffect()
    {
        if (activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(activeWeapon.GetCurrentWeapon().weaponDetails.weaponFiringSoundEffect);
        }
    }
}
