using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region Require
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MaterializeEffect))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(ReloadWeapon))]

[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(Idle))]

#endregion
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public EnemyDetailsSO enemyDetails;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    private SetActiveWeaponEvent setActiveWeaponEvent;
    private FireWeapon fireWeapon;

    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;
    [HideInInspector] public MaterializeEffect materializeEffect;

    [HideInInspector] public EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        materializeEffect = GetComponent<MaterializeEffect>();

        fireWeapon = GetComponent<FireWeapon>();

        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();

        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
    }

    public void EnemyInitialization(EnemyDetailsSO enemyDetails,int enemySpawnNumber,DungeonLevelSO dungeonLevel)
    {
        this.enemyDetails = enemyDetails;

        SetEnemyMovementUpdateFrame(enemySpawnNumber);

        SetEnemyStartingWeapon();

        SetEnemyAnimationSpeed();

        StartCoroutine(MaterializeEnemy());

    }


    private void SetEnemyMovementUpdateFrame(int enemySpawnNumber)
    {
        enemyMovementAI.SetUpdateFrameNumber(enemySpawnNumber);
    }

    private void SetEnemyStartingWeapon()
    {
        if (enemyDetails.enemyWeapon != null)
        {
            Weapon weapon = new Weapon()
            {
                weaponDetails = enemyDetails.enemyWeapon,
                weaponReloadTimer = 0f,
                weaponClipRemainingAmmo = enemyDetails.enemyWeapon.weaponClipAmmoCapacity,
                weaponRemainAmmo = enemyDetails.enemyWeapon.weaponAmmoCapacity,
                isWeaponReloading = false
            };
            setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }


    private void SetEnemyAnimationSpeed()
    {
        animator.speed = enemyMovementAI.moveSpeed / Settings.baseSpeedForEnemyAnimations;
    }

    private IEnumerator MaterializeEnemy()
    {
        EnemyEnable(false);
        yield return StartCoroutine(materializeEffect.MaterializeRoutine(enemyDetails.enemyMaterializeShader, enemyDetails.enemyMaterializeColor,
            enemyDetails.enemyMaterializeTime, spriteRendererArray, enemyDetails.enemyStandardMaterial));
        EnemyEnable(true);
    }

    private void EnemyEnable(bool isEnabled)
    {
        circleCollider2D.enabled = isEnabled;
        polygonCollider2D.enabled = isEnabled;

        enemyMovementAI.enabled = isEnabled;

        fireWeapon.enabled = isEnabled;
    }
}
