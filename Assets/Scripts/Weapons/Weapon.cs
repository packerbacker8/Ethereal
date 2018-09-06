using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public abstract class Weapon : MonoBehaviour
{
    public string weaponName = "AK47";
    public string weaponSlot = "primary";

    public int damage = 36;
    public float armorPenetration = 0.5f;
    public float range = 1000f; //TODO: might want damage falloff, might not
    public float roundsPerMinute = 600f; //fires once every tenth of a second
    public float timeToReload = 2.5f; //2 and half seconds
    public float settleTime = 0.25f; // every quarter a second go back one index on the spray if not shooting
    public float sellBackRatio = 0.5f;

    public int ammo = 30;
    public int currentAmmo = 30;
    public int totalAmmo = 240;
    public int cost = 2700;
    public int killValue = 300;
    public int teamValue = 100;

    public GameObject weaponGraphics;
    public Sprite uiIcon;

    public float[] SprayPatternX
    {
        get
        {
            return sprayPatternX;
        }
        protected set
        {
            sprayPatternX = value;
        }
    }

    public float[] SprayPatternY
    {
        get
        {
            return sprayPatternY;
        }
        protected set
        {
            sprayPatternY = value;
        }
    }

    public float[] MovingSprayPatternX
    {
        get
        {
            return movingSprayPatternX;
        }
        protected set
        {
            movingSprayPatternX = value;
        }
    }

    public float[] MovingSprayPatternY
    {
        get
        {
            return movingSprayPatternY;
        }
        protected set
        {
            movingSprayPatternY = value;
        }
    }

    public float[] CrouchingSprayPatternX
    {
        get
        {
            return crouchingSprayPatternX;
        }
        protected set
        {
            crouchingSprayPatternX = value;
        }
    }

    public float[] CrouchingSprayPatternY
    {
        get
        {
            return crouchingSprayPatternY;
        }
        protected set
        {
            crouchingSprayPatternY = value;
        }
    }

    protected float[] sprayPatternX;
    protected float[] sprayPatternY;
    protected float[] movingSprayPatternX;
    protected float[] movingSprayPatternY;
    protected float[] crouchingSprayPatternX;
    protected float[] crouchingSprayPatternY;

    private float rateOfFire;

    protected virtual void Start()
    {
        rateOfFire = 1 / (roundsPerMinute / 60);
        SetupWeapon();
    }

    /// <summary>
    /// Describes how long between each bullet it takes in seconds.
    /// </summary>
    /// <returns>Calculated by: 1 / (roundsPerMinute / 60)</returns>
    public float GetRateOfFireTime()
    {
        return rateOfFire;
    }



    /// <summary>
    /// Where values are set that are specific to each individual weapon and some 
    /// type specific values where applied to the entire weapon type.
    /// </summary>
    public virtual void SetupWeapon()
    {
        this.gameObject.name = weaponName;
    }
}
