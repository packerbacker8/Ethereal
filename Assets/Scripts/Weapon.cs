using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Weapon : MonoBehaviour
{
    public string weaponName = "AK-47";

    public float damage = 27f;
    public float armorPenetration = 0.5f;
    public float range = 1000f; //TODO: might want damage falloff, might not
    public float rateOfFire = 0.25f; //fires once every quarter second

    public float[] sprayPatternX = new float[] { 0, 0, 0.1f};
    public float[] sprayPatternY;
    public float[] movingSprayPatternX;
    public float[] movingSprayPatternY;

    public int ammo = 30;
    public int totalAmmo = 240;
}
