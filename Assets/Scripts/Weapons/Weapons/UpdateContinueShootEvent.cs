using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateContinueShootEvent : MonoBehaviour
{
    public Action<bool> OnUpdateContinueShootEvent;

    public void CallUpdateContinueShootEvent(bool target)
    {
        OnUpdateContinueShootEvent?.Invoke(target);
    }
}
