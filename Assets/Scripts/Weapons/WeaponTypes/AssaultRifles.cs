using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifles : Weapon
{
    public string WeaponType = "Assault Rifle";

    protected override void Start()
    {
        base.Start();
    }

    public override void SetupWeapon()
    {
        killValue = 300;
        teamValue = 100;
    }
}
