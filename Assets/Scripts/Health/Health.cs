using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    [HideInInspector] public bool isDamagebale = true;
    private Player player;
    [HideInInspector] public Enemy enemy;

    [Header("Immunity")]
    private bool isImmunityAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(spriteFlashInterval);
    public Coroutine immunityCoroutine;
    [SerializeField] private HealthBar healthBar;


    private void Awake()
    {
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {

        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        if (player != null)
        {
            if (player.playerDetails.isImmuneAfterHit)
            {
                isImmunityAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        else if (enemy != null)
        {
            if (enemy != null)
            {
                if (enemy.enemyDetails.isImmuneAfterHit)
                {
                    isImmunityAfterHit = true;
                    immunityTime = enemy.enemyDetails.hitImmunityTime;
                    spriteRenderer = enemy.spriteRendererArray[0];
                }
            }
        }

        if (enemy != null && enemy.enemyDetails.isHealthBarDisplayed == true && healthBar != null)
        {
            healthBar.EnableHealthBar();
        }
        else if(healthBar!=null)
        {
            healthBar.DisableHealthBar();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        bool isPlayerRolling = false;
        if (player!= null)
        {
            isPlayerRolling = player.playerControl.isPlayerRolling;
        }
        if (isDamagebale&&!isPlayerRolling)
        {
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);

            PostHitImmunity();
            if (healthBar != null)
            {
                healthBar.SetHealthBarValue((float)currentHealth / (float)startingHealth);
            }
        }
    }

    private void PostHitImmunity()
    {
        if (gameObject.activeSelf == false)
        {
            return;
        }
        if (isImmunityAfterHit)
        {
            if (immunityCoroutine != null)
            {
                StopCoroutine(immunityCoroutine);
            }
            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));

        }
    }

    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);

        isDamagebale = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;
            yield return waitForSeconds;
            spriteRenderer.color = Color.white;
            yield return waitForSeconds;
            iterations--;
            yield return null;
        }
        isDamagebale = true;

    }

    private void CallHealthEvent(int damageAmount)
    {
        healthEvent.CallHealthChangedEvent(((float)currentHealth / (float)startingHealth), currentHealth, damageAmount);
    }

    public void SetStartingHealth(int health)
    {
        this.startingHealth = health;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }
}
