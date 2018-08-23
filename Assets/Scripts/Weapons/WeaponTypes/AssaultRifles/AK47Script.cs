using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47Script : AssaultRifles
{
    public override void SetupWeapon()
    {
        base.SetupWeapon();
        sprayPatternX = new float[] { 0, 0, 0.1f, -0.15f, 0.11f, 0.12f, -0.23f, 0.29f, 0.32f, 0.4f, 0.31f, 0.32f, 0.35f, 0.4f, 0.33f, 0.218f, 0.18f, 0.07f, -0.22f, -0.3f, -0.45f, -0.14f, 0.16f, 0.24f, 0.53f, 0.12f, -0.15f, 0.09f, -0.31f, -0.09f };
        sprayPatternY = new float[] { 0, 0, 0.1f, 0.15f, 0.18f, 0.22f, 0.14f, 0.24f, 0.28f, 0.29f, 0.35f, 0.37f, 0.35f, 0.41f, 0.42f, 0.5f, 0.513f, 0.47f, 0.446f, 0.45f, 0.49f, 0.41f, 0.33f, 0.4f, 0.46f, 0.5f, 0.52f, 0.43f, 0.47f, 0.47f };
    }
}
