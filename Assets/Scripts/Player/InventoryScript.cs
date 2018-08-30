using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    private Dictionary<string, GameObject> inventory;

    private void Start()
    {
        inventory = new Dictionary<string, GameObject>() {
            { "primary", null},
            { "secondary", null},
            { "knife", null},
            { "grenades", null},
            { "bomb", null}
        };

    }

    public Dictionary<string, GameObject> GetInventory()
    {
        return inventory;
    }

    public GameObject GetPrimaryWeapon()
    {
        return inventory["primary"];
    }

    public GameObject GetSecondaryWeapon()
    {
        return inventory["secondary"];
    }

    public GameObject GetKnife()
    {
        return inventory["knife"];
    }

    public GameObject GetGrenades()
    {
        return inventory["grenades"];
    }

    public GameObject GetObjectiveBomb()
    {
        return inventory["bomb"];
    }

    public bool SetPrimaryWeapon(GameObject wep)
    {
        return false;
    }
}
