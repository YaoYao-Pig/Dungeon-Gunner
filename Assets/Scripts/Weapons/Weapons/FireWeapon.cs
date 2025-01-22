using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(FireWeaponEvent))]
[DisallowMultipleComponent]
public class FireWeapon : MonoBehaviour
{
    private float fireRateCoolDownTimer = 0f;
    private float firePreChargTimer = 0f;

    private ActiveWeapon activeWeapon;
    private FireWeaponEvent fireWeaponEvent;
    private WeaponFiredEvent weaponFiredEvent;

    private ReloadWeaponEvent reloadWeaponEvent;
    private void Awake()
    {
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
    }
    private void OnEnable()
    {
        fireWeaponEvent.OnFireWeapon += FireWeaponEvent_OnFireWeapon;
    }
    private void OnDisable()
    {
        fireWeaponEvent.OnFireWeapon -= FireWeaponEvent_OnFireWeapon;
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
            GameObject ammoPrefab = currentAmmo.ammoPrefabArray[UnityEngine.Random.Range(0, currentAmmo.ammoPrefabArray.Length)];
            float ammoSpeed = UnityEngine.Random.Range(currentAmmo.ammoSpeedMin, currentAmmo.ammoSpeedMax);
            IFireable ammo = PoolManager.Instance.ReuseComponent(ammoPrefab, activeWeapon.GetShootPosition(), Quaternion.identity) as IFireable;
            if (ammo == null)
            {
                Debug.LogError("Don't have IFireable in Pool Manager");
                return;
            }

            ammo.InitialiseAmmo(currentAmmo, aimAngle, weaponAimAngle, ammoSpeed, weaponAimDirectionVector);

            if (!activeWeapon.GetCurrentWeapon().weaponDetails.hasInfiniteClipCapacity)
            {
                activeWeapon.GetCurrentWeapon().weaponClipRemainingAmmo--;
                activeWeapon.GetCurrentWeapon().weaponRemainAmmo--;
            }
            weaponFiredEvent.CallWeaponFiredEvenet(activeWeapon.GetCurrentWeapon());
        }
    }


    private void ResetCoolDownTimer()
    {
        fireRateCoolDownTimer = activeWeapon.GetCurrentWeapon().weaponDetails.weaponFireRate;
    }
}
