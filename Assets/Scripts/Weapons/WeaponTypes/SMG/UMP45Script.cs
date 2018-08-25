﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UMP45Script : SMG
{
    public override void SetupWeapon()
    {
        base.SetupWeapon();
        sprayPatternX = new float[] { 0.006134987f, -0.003681004f, -0.00858897f, -0.003681004f, 0.01104295f, 0.01595092f, -0.001227021f, -0.0184049f, -0.03312886f, -0.06257671f, -0.04049081f, 0.02331293f, -0.01349694f, -0.07730061f, -0.1092024f,-0.05030674f, 0.01595092f, -0.003681004f, -0.04294479f, -0.001227021f, 0.01349688f, -0.01104295f, 0.01349688f, 0.04539883f, 0.03312886f };
        sprayPatternY = new float[] { 0.005780339f, 0.03757226f, 0.06069362f, 0.1011561f, 0.1416185f, 0.1734104f, 0.1705202f, 0.1589595f, 0.1676301f, 0.1763006f, 0.1907514f, 0.1907514f, 0.1994219f, 0.1965318f, 0.2138728f, 0.2196532f, 0.216763f, 0.2398844f, 0.2398844f, 0.2485549f, 0.2803468f, 0.2919075f, 0.3121387f, 0.3265896f, 0.3583815f};
        movingSprayPatternX = new float[] {0.00497508f, 0.008291841f, -0.0082919f, -0.0199005f, -0.0082919f, 0.0232172f, 0.0116086f, -0.03316748f, 0.00995028f, -0.00497514f, -0.0199005f, 0.0232172f, -0.00331676f, -0.03648424f, -0.0597015f, -0.09452736f, -0.1276948f, -0.1227197f, -0.06965172f, -0.00663352f, 0.04145932f, 0.07296848f, 0.09452736f, 0.06467664f, 0.07296848f };
        movingSprayPatternY = new float[] { 0.006075323f, 0.04009718f, 0.0473876f, 0.0838396f, 0.1081409f, 0.1130012f, 0.1445929f, 0.1494532f, 0.1713244f, 0.1907655f, 0.1931956f, 0.2150669f, 0.2150669f, 0.217497f, 0.2126367f, 0.1761847f, 0.1664641f, 0.1931956f, 0.2247874f, 0.2393681f, 0.2466586f, 0.2272175f, 0.1883354f, 0.2077764f, 0.2733901f};
    }
}
