using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : Weapon
{
    public string WeaponType = "SMG";

    protected override void Start()
    {
        base.Start();
    }

    public override void SetupWeapon()
    {
        killValue = 400;
        teamValue = 150;
    }
}
