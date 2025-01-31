using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour, IFireable
{
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f;
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;
    private bool isColliding = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    ///Ammo发射之后，首先是ChargeTime充能
    ///时间到了之后判断是否要更新材质
    ///然后每帧更新位置
    ///如果超出最远距离就销毁
    ///如果有碰撞也销毁
    private void Update()
    {
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = true;
        }
        if (!overrideAmmoMovement)
        {
            Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;
            transform.position += distanceVector;

            ammoRange -= distanceVector.magnitude;
            if (ammoRange <= 0f)
            {
                if (ammoDetails.isPlayerAmmo)
                {
                    StaticEventHandler.CallMultiplierEvent(false);
                }
                DisableAmmo();
            }
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isColliding) return;

        DealDamage(collision);
        AmmoHitEffect();
        DisableAmmo();
    }

    private void DealDamage(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();
        bool enemyHit = false;
        if (health != null)
        {
            isColliding = true;
            health.TakeDamage(ammoDetails.ammoDamage);
            if (health.enemy != null)
            {
                enemyHit = true;
            }
        }
        if (ammoDetails.isPlayerAmmo)
        {
            StaticEventHandler.CallMultiplierEvent(enemyHit);

        }

    }

    private void AmmoHitEffect()
    {
        if (ammoDetails.ammoHitEffect != null && ammoDetails.ammoHitEffect.ammoHitEffectPrefab != null)
        {
            AmmoHitEffect ammoHitEffect = (AmmoHitEffect)PoolManager.Instance.ReuseComponent(ammoDetails.ammoHitEffect.ammoHitEffectPrefab, transform.position, Quaternion.identity);

            ammoHitEffect.SetHitEffect(ammoDetails.ammoHitEffect);
            ammoHitEffect.gameObject.SetActive(true);
        }
    }

    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    private void SetAmmoMaterial(Material ammoChargeMaterial)
    {
        spriteRenderer.material = ammoChargeMaterial;
    }

    //会被PoolManager回收，因此所有变量包括bool需要初始化
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;
        isColliding = false;
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        spriteRenderer.sprite = ammoDetails.ammoSprite;

        if (ammoDetails.ammoChargeTime > 0f)
        {
            this.ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        this.ammoRange = ammoDetails.ammoRange;
        this.ammoSpeed = ammoSpeed;

        this.overrideAmmoMovement = overrideAmmoMovement;

        gameObject.SetActive(true);
        #endregion

        #region Trail
        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoTrailMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }
        #endregion

    }

    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {

        float randomSpread = UnityEngine.Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        int spreadToggle = UnityEngine.Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        fireDirectionAngle += spreadToggle * randomSpread;
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        this.fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }


}
