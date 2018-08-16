using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Weapon : MonoBehaviour
{
    public string weaponName = "AK-47";

    public float damage = 36f;
    public float armorPenetration = 0.5f;
    public float range = 1000f; //TODO: might want damage falloff, might not
    public float roundsPerMinute = 600f; //fires once every tenth of a second
    public float timeToReload = 2.5f; //2 and half seconds
    public float settleTime = 0.25f; // every quarter a second go back one index on the spray if not shooting

    public int ammo = 30;
    public int totalAmmo = 240;

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

    private float[] sprayPatternX = new float[] { 0, 0, 0.1f, -0.15f, 0.11f, 0.12f, -0.23f, 0.29f, 0.32f, 0.4f, 0.31f, 0.32f, 0.35f, 0.4f, 0.33f, 0.218f, 0.18f, 0.07f, -0.22f, -0.3f, -0.45f, -0.14f, 0.16f, 0.24f, 0.53f, 0.12f, -0.15f, 0.09f, -0.31f, -0.09f};
    private float[] sprayPatternY = new float[] { 0, 0, 0.1f, 0.15f, 0.18f, 0.22f, 0.14f, 0.24f, 0.28f, 0.29f, 0.35f, 0.37f, 0.35f, 0.41f, 0.42f, 0.5f, 0.513f, 0.47f, 0.446f, 0.45f, 0.49f, 0.41f, 0.33f, 0.4f, 0.46f, 0.5f, 0.52f, 0.43f, 0.47f, 0.47f };
    private float[] movingSprayPatternX;
    private float[] movingSprayPatternY;
    private float[] crouchingSprayPatternX;
    private float[] crouchingSprayPatternY;

    private float rateOfFire;

    private void Start()
    {
        rateOfFire = 1 / (roundsPerMinute / 60);
    }

    /// <summary>
    /// Describes how long between each bullet it takes in seconds.
    /// </summary>
    /// <returns>Calculated by: 1 / (roundsPerMinute / 60)</returns>
    public float GetRateOfFireTime()
    {
        return rateOfFire;
    }
}
