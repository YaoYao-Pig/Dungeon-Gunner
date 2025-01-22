using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeaponDetails_",menuName ="Scriptable Objects/Weapons/Weapons Details")]
public class WeaponDetailsSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;

    public Vector3 weaponShootPosition;

    public AmmoDetailsSO weaponCurrentAmmo;

    public bool hasInfiniteAmmo = false;    //是否无限子弹
    public bool hasInfiniteClipCapacity = false;    //是否子弹容量无限
    public bool canContinueFire = true; //是否能连续射击
    public int weaponClipAmmoCapacity = 6;  //每个弹夹容量
    public int weaponAmmoCapacity = 100;    //子弹容量

    public float weaponFireRate = 0.2f;  //开火频率

    public float weaponRechargeTime = 0f;   //开火延迟

    public float weaponReloadTime = 0f;


#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponFireRate), weaponFireRate,false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRechargeTime), weaponRechargeTime,true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponAmmoCapacity), weaponAmmoCapacity, false);
        }
        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponClipAmmoCapacity), weaponClipAmmoCapacity, false);
        }

    }
#endif
}
