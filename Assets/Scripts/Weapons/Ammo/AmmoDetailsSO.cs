using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/Ammo Details")]
public class AmmoDetailsSO : ScriptableObject
{
    public string ammoName;
    public bool isPlayerAmmo;

    public Sprite ammoSprite;

    public GameObject[] ammoPrefabArray;

    public Material ammoMaterial;


    public float ammoChargeTime = 0.1f; //�ӵ������ԭ��ͣ��ʱ��
    public Material ammoChargeMaterial;

    [Header("AMMO BASE PARAMETERS")]
    public int ammoDamage = 1;
    public float ammoSpeedMin = 20f;
    public float ammoSpeedMax = 20f;
    public float ammoRange = 20f;
    public float ammoRotationSpeed = 1f;

    public AmmoHitEffectSO ammoHitEffect;

    [Header("AMMO SPREAD DETAILS")]
    //�ӵ���ɢ
    public float ammoSpreadMin = 0f;
    public float ammoSpreadMax = 0f;
    [Header("AMMO SPAWN DETAILS")]
    //��ҩ��������
    public int ammoSpawnAmountMin = 1;
    public int ammoSpawnAmountMax = 1;

    public float ammoSpawnIntervalMin = 0f;
    public float ammoSpawnIntervalMax = 0f;

    [Header("AMMO TRAIL DETAILS")]
    public bool isAmmoTrail = false;
    public float ammoTrailTime = 3f;

    public Material ammoTrailMaterial;
    [Range(0f, 1f)] public float ammoTrailStartWidth;
    [Range(0f, 1f)] public float ammoTrailEndWidth;



}
