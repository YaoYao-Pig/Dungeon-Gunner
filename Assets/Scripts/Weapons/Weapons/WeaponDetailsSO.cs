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

    public bool hasInfiniteAmmo = false;    //�Ƿ������ӵ�
    public bool hasInfiniteClipCapacity = false;    //�Ƿ��ӵ���������
    public bool canContinueFire = true; //�Ƿ����������
    public int weaponClipAmmoCapacity = 6;  //ÿ����������
    public int weaponAmmoCapacity = 100;    //�ӵ�����

    public float weaponFireRate = 0.2f;  //����Ƶ��

    public float weaponRechargeTime = 0f;   //�����ӳ�

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
