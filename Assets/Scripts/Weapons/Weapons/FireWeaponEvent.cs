using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FireWeaponEvent : MonoBehaviour
{
    public Action<FireWeaponEvent, FireWeaponEventArgs> OnFireWeapon;

    public void CallFireaWeaponEvent(bool fire,bool firePreviousFrame,AimDirection aimDirection,float aimAngle,float weaponAimAngle,Vector3 weaponAimDirectionVector)
    {
        OnFireWeapon?.Invoke(this,new FireWeaponEventArgs()
        {
            fire = fire,
            firePreviousFrame= firePreviousFrame,
            aimDirection = aimDirection,
            aimAngle = aimAngle,
            weaponAimAngle = weaponAimAngle,
            weaponAimDirectionVector = weaponAimDirectionVector
        });
    }
}

public class FireWeaponEventArgs : EventArgs
{
    public bool fire;
    public bool firePreviousFrame;
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}
