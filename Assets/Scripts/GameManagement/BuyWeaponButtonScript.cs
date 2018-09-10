using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyWeaponButtonScript : MonoBehaviour
{
    /// <summary>
    /// This weapon name is to match the weapon name found in the variable stored on the weapon
    /// script of the prefabed object.
    /// </summary>
    public string weaponName;

    private PlayerUIScript ui;

    private void Start()
    {
        ui = this.transform.parent.parent.GetComponent<PlayerUIScript>();
        if (ui == null)
        {
            Debug.LogError("No UI script found in this BuyWeaponButtonScript: " + this.name);
        }
    }

    /// <summary>
    /// Tells the UI which weapon the player is trying to buy
    /// so that it may pick the appropriate one to spawn.
    /// </summary>
    public void BuyThisPersonalWeapon()
    {
        ui.BuyPersonalWeapon(weaponName);
    }

    public void BuyThisTeamItem()
    {

    }

    public void BuyThisGrenade()
    {

    }
}
