using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveContactDamage : MonoBehaviour
{
    [SerializeField] private int contactDamageAmount;//可以通过给这里的contactDamageAmount赋值，覆盖收到的伤害，用于护甲
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeContactDamage(int damageAmount = 0)
    {
        if (contactDamageAmount > 0)
        {
            damageAmount = contactDamageAmount;
        }
        health.TakeDamage(damageAmount);
    }
}
