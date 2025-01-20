using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable 
{
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed,
                                Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false);
    public GameObject GetGameObject();


}
 