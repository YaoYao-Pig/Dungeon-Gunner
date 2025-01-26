using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyEvent : MonoBehaviour
{
    public event Action<DestroyEvent, DestroyEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied)
    {
        OnDestroyed?.Invoke(this,new DestroyEventArgs() { playerDied= playerDied });
    }
}

public class DestroyEventArgs : EventArgs
{
    public bool playerDied;
} 
