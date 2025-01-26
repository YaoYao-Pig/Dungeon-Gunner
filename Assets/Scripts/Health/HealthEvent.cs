using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEvent : MonoBehaviour
{
    public event Action<HealthEvent, HealthEventArgs> OnHealthChanged;
    public void CallHealthChangedEvent(float healthPrecent,int healthAmount,int damageAmount)
    {
        OnHealthChanged?.Invoke(this, new HealthEventArgs()
        {
            healthPrecent= healthPrecent,
            healthAmount=healthAmount,
            damageAmount=damageAmount
        });
    }
}
public class HealthEventArgs : EventArgs
{
    public float healthPrecent;
    public int healthAmount;
    public int damageAmount;
}