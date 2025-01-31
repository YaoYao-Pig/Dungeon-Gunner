using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyableItem : MonoBehaviour
{
    [SerializeField] private int startingHealthAmount = 1;
    [SerializeField] private SoundEffectSO destroySoundEffect;
    private Animator animator;
    private BoxCollider2D boxCollider2D;
    private HealthEvent healthEvent;
    private Health health;
    private ReceiveContactDamage receiveContactDamage;
    [SerializeField] private Sprite brokenSprite;
    private SpriteRenderer spriteRenderer;
    private bool hasTrigger = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healthEvent = GetComponent<HealthEvent>();
        health = GetComponent<Health>();
        health.SetStartingHealth(startingHealthAmount);
        receiveContactDamage = GetComponent<ReceiveContactDamage>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTrigger == false)
        {
            hasTrigger = true;
            StartCoroutine(PlayAnimation());
        }
    }
    private IEnumerator PlayAnimation()
    {

        Debug.Log(name + "1");
        Destroy(boxCollider2D);

        if (destroySoundEffect != null)
        {
            SoundEffectManager.Instance.PlaySoundEffect(destroySoundEffect);
        }

        animator.SetBool(Settings.destroy,true);


        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(Settings.stateDestroyed))
        {
            yield return null;
        }

        Destroy(animator);
        Destroy(receiveContactDamage);
        Destroy(health);
        Destroy(healthEvent);
        //spriteRenderer.sprite = brokenSprite;
        Destroy(this);
    }
}
