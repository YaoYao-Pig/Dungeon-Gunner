using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    public string enemyName;

    public GameObject enemyPrefab;

    public float chaseDistance = 50f;

    [Header("Materialize")]
    public Material enemyStandardMaterial;
    public float enemyMaterializeTime;
    public Shader enemyMaterializeShader;
    public Color enemyMaterializeColor;
    [Header("Weapon")]
    public WeaponDetailsSO enemyWeapon;

    //间隔
    public float firingIntervalMin = 0.1f;
    public float firingIntervalMax = 1f;
    //持续时间
    public float firingDurationMin = 1f;
    public float firingDurationMax = 1f;

    //是否需要无遮挡
    public bool firingLineofSightRequired;


}
