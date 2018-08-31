using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grenades : NetworkBehaviour
{
    public string WeaponType = "Grenade";

    public string weaponName = "Frag Grenade";
    public string weaponSlot = "grenades";

    public int damage = 74;
    public float armorPenetration = 0.1f;
    public float damageRadius = 100f;
    public float throwDistance = 100f;

    public int killValue = 500;
    public int teamValue = 50;
    public int numAbleToEquip = 1;

    public GameObject weaponGraphics;

    protected virtual void Start()
    {
        SetupWeapon();
    }

    /// <summary>
    /// Where values are set that are specific to each individual weapon and some 
    /// type specific values where applied to the entire weapon type.
    /// </summary>
    public virtual void SetupWeapon()
    {
        
    }
}
