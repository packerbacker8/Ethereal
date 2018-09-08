﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47Script : AssaultRifles
{
    public override void SetupWeapon()
    {
        sprayPatternX = new float[] { 0, 0, 0.001227021f, 0.001227021f, 0.00858891f, 0.01595092f, 0.02085888f, 0.03803682f, 0.06503069f, 0.1263803f, 0.1558282f, 0.08466256f, -0.001227021f, -0.07975459f, -0.02331287f,-0.08711654f, 0.03803682f, 0.07975459f, -0.01349694f, -0.08711654f, 0.02822089f, -0.0552147f, 0.00858891f, -0.03558284f, -0.01104295f, -0.06503069f, 0.001227021f, 0.04294479f, 0.0871166f, 0.1141105f };
        sprayPatternY = new float[] { 0f, 0f, 0.01445085f, 0.04046243f, 0.06069362f, 0.0780347f, 0.08670521f, 0.1040462f, 0.1098266f, 0.115607f, 0.1184971f, 0.1300578f, 0.1242775f, 0.1271676f, 0.1445087f, 0.1416185f, 0.1618497f, 0.1647399f, 0.1734104f, 0.1734104f, 0.1763006f, 0.1763006f, 0.1878613f, 0.1878613f, 0.1878613f, 0.1965318f, 0.1994219f, 0.1994219f, 0.1965318f, 0.1965318f};

        settlePatternX = new float[sprayPatternX.Length];
        settlePatternY = new float[sprayPatternY.Length];
        for (int i = 0; i < settlePatternX.Length; i++)
        {
            settlePatternX[i] = sprayPatternX[i] * camSettleAmount * -1;
            settlePatternY[i] = sprayPatternY[i] * camSettleAmount * -1;
        }

        movingSprayPatternX = new float[] { -0.03067487f, -0.05030674f, -0.03558284f, 0.01104295f, 0.06748462f, 0.0871166f, 0.04294479f, 0.08220863f, 0.0871166f, 0.01595092f, -0.04785275f, -0.1165644f, -0.1509203f, -0.2f, -0.1386503f, -0.06993866f, -0.01595092f, 0.04294479f, -0.08711654f, -0.1558282f, -0.0184049f, 0.0503068f, 0.08957052f, 0.1312883f, 0.1779141f, 0.0871166f, 0.01595092f, -0.04785275f, -0.1165644f, -0.1656442f };
        movingSprayPatternY = new float[] { 0.05444127f, 0.08022922f, 0.1203439f, 0.1346705f, 0.1805158f, 0.2206303f, 0.2320917f, 0.269341f, 0.3209169f, 0.3037249f, 0.3008596f, 0.3008596f, 0.3467049f, 0.3638968f, 0.3810889f, 0.3896848f, 0.3839542f, 0.3896848f, 0.4126074f, 0.4297994f, 0.4584527f, 0.4727794f, 0.4985673f, 0.5157593f, 0.5071633f, 0.5186247f, 0.5186247f, 0.512894f, 0.4928367f, 0.4899713f };
        base.SetupWeapon();
    }
}
