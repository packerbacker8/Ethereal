using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    [Header("Inventory Slot Keys")]
    public const string PRIMARY = "primary";
    public const string SECONDARY = "secondary";
    public const string KNIFE = "knife";
    public const string GRENADES = "grendades";
    public const string BOMB = "bomb";

    public const string WEAPON_LAYER = "Weapon";
    public const string WORLD_OBJECT_LAYER = "WorldObject";

    private Dictionary<string, GameObject> inventory;

    public void SetupInventory()
    {
        inventory = new Dictionary<string, GameObject>() {
            { PRIMARY, null},
            { SECONDARY, null},
            { KNIFE, null},
            { GRENADES, null},
            { BOMB, null}
        };

        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;

            if(child.GetComponent<Weapon>() != null)
            {
                child.GetComponent<Rigidbody>().useGravity = false;
                child.GetComponent<Rigidbody>().isKinematic = true;
                child.GetComponent<Rigidbody>().detectCollisions = false;
                child.GetComponent<Collider>().enabled = false;
                inventory[child.GetComponent<Weapon>().weaponSlot] = child;

            }
            else if(child.GetComponent<Grenades>() != null)
            {
                child.GetComponent<Rigidbody>().useGravity = false;
                child.GetComponent<Rigidbody>().isKinematic = true;
                child.GetComponent<Rigidbody>().detectCollisions = false;
                child.GetComponent<Collider>().enabled = false;
                inventory[child.GetComponent<Grenades>().weaponSlot] = child;
            }
        }
    }

    public List<string> GetAllInventorySlotNames()
    {
        return new List<string>() { PRIMARY, SECONDARY, KNIFE, GRENADES, BOMB };
    }

    public Dictionary<string, GameObject> GetInventory()
    {
        return inventory;
    }

    public GameObject GetWeapon(string slot)
    {
        switch (slot)
        {
            case PRIMARY:
                return GetPrimaryWeapon();
            case SECONDARY:
                return GetSecondaryWeapon();
            case KNIFE:
                return GetKnife();
            case GRENADES:
                return GetGrenades();
            case BOMB:
                return GetObjectiveBomb();
            default:
                Debug.LogError("This is not a recognized inventory slot: " + slot);
                return null;
        }
    }

    public GameObject GetPrimaryWeapon()
    {
        return inventory[PRIMARY];
    }

    public GameObject GetSecondaryWeapon()
    {
        return inventory[SECONDARY];
    }

    public GameObject GetKnife()
    {
        return inventory[KNIFE];
    }

    public GameObject GetGrenades()
    {
        return inventory[GRENADES];
    }

    public GameObject GetObjectiveBomb()
    {
        return inventory[BOMB];
    }

    public void RemoveWeapon(string slot)
    {
        GameObject wepToRemove;
        switch (slot)
        {
            case PRIMARY:
                wepToRemove = inventory[PRIMARY];
                inventory[PRIMARY] = null;
                break;
            case SECONDARY:
                wepToRemove = inventory[SECONDARY];
                inventory[SECONDARY] = null;
                break;
            case KNIFE:
                wepToRemove = inventory[KNIFE];
                inventory[KNIFE] = null;
                break;
            case GRENADES:
                wepToRemove = inventory[GRENADES];
                inventory[GRENADES] = null;
                break;
            case BOMB:
                wepToRemove = inventory[BOMB];
                inventory[BOMB] = null;
                break;
            default:
                Debug.LogError("This is not a recognized inventory slot: " + slot);
                return;
        }
        wepToRemove.transform.parent = null;
        Util.SetLayerRecusively(wepToRemove, LayerMask.NameToLayer(WORLD_OBJECT_LAYER));
        wepToRemove.transform.position = wepToRemove.transform.position + wepToRemove.transform.forward * (this.GetComponent<SphereCollider>().radius * 2.5f);
        wepToRemove.GetComponent<Rigidbody>().useGravity = true;
        wepToRemove.GetComponent<Rigidbody>().isKinematic = false;
        wepToRemove.GetComponent<Rigidbody>().detectCollisions = true;
        wepToRemove.GetComponent<Rigidbody>().AddForce(wepToRemove.transform.forward * 100f);
        wepToRemove.GetComponent<Rigidbody>().AddTorque(wepToRemove.transform.right * 100f);
        wepToRemove.GetComponent<Collider>().enabled = true;
        this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
    }

    public void ClearInventory()
    {
        inventory = new Dictionary<string, GameObject>() {
            { PRIMARY, null},
            { SECONDARY, null},
            { KNIFE, null},
            { GRENADES, null},
            { BOMB, null}
        };
    }

    public bool SetWeapon(string slot, GameObject wep)
    {
        switch (slot)
        {
            case PRIMARY:
                if (SetPrimaryWeapon(wep))
                {
                    wep.transform.SetParent(this.transform);
                    this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
                    return true;
                }
                else
                {
                    return false;
                }
            case SECONDARY:
                if (SetSecondaryWeapon(wep))
                {
                    wep.transform.SetParent(this.transform);
                    this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
                    return true;
                }
                else
                {
                    return false;
                }
            case KNIFE:
                if (SetKnifeWeapon(wep))
                {
                    wep.transform.SetParent(this.transform);
                    this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
                    return true;
                }
                else
                {
                    return false;
                }
            case GRENADES:
                if (SetGrenades(wep))
                {
                    wep.transform.SetParent(this.transform);
                    this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
                    return true;
                }
                else
                {
                    return false;
                }
            case BOMB:
                if (SetObjectiveBomb(wep))
                {
                    wep.transform.SetParent(this.transform);
                    this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                Debug.LogError("This is not a recognized inventory slot: " + slot);
                return false;
        }
    }

    private bool SetPrimaryWeapon(GameObject wep)
    {
        if(wep == null || wep.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if(inventory[PRIMARY] != null)
        {
            RemoveWeapon(PRIMARY);
        }
        inventory[PRIMARY] = wep;
        return true;
    }

    private bool SetSecondaryWeapon(GameObject wep)
    {
        if (wep == null || wep.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if (inventory[SECONDARY] != null)
        {
            RemoveWeapon(SECONDARY);
        }
        inventory[SECONDARY] = wep;
        return true;
    }

    private bool SetKnifeWeapon(GameObject wep)
    {
        if (wep == null || wep.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if (inventory[KNIFE] != null)
        {
            RemoveWeapon(KNIFE);
        }
        inventory[KNIFE] = wep;
        return true;
    }

    private bool SetGrenades(GameObject grenades)
    {
        if (grenades == null || grenades.GetComponent<Grenades>() == null)
        {
            return false;
        }
        if (inventory[GRENADES] != null)
        {
            RemoveWeapon(GRENADES);
        }
        inventory[GRENADES] = grenades;
        return true;
    }

    private bool SetObjectiveBomb(GameObject bomb)
    {
        if (bomb == null || bomb.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if (inventory[BOMB] != null)
        {
            RemoveWeapon(BOMB);
        }
        inventory[BOMB] = bomb;
        return true;
    }

    /// <summary>
    /// Collider that is being used to pickup weapons.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponentInChildren<Weapon>() != null)
        {
            PickupWeapon(col.gameObject);
        }
        else if (col.gameObject.GetComponentInChildren<Grenades>() != null)
        {
            PickupGrenade(col.gameObject);
        }
    }

    /// <summary>
    /// If the weapon in the slot that it should go in, is not currently taken, place this weapon obj there.
    /// Else do nothing with it.
    /// </summary>
    /// <param name="weaponToPickup"></param>
    private void PickupWeapon(GameObject weaponToPickup)
    {
        Weapon wepInfo = weaponToPickup.GetComponent<Weapon>();
        if(inventory[wepInfo.weaponSlot] == null)
        {
            weaponToPickup.transform.position = this.transform.position;
            weaponToPickup.transform.rotation = this.transform.rotation;
            weaponToPickup.transform.parent = this.transform;
            weaponToPickup.GetComponent<Rigidbody>().useGravity = false;
            weaponToPickup.GetComponent<Rigidbody>().isKinematic = true;
            weaponToPickup.GetComponent<Rigidbody>().detectCollisions = false;
            weaponToPickup.GetComponent<Collider>().enabled = false;

            inventory[wepInfo.weaponSlot] = weaponToPickup;

            this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
        }
    }

    /// <summary>
    /// If the grenade in the slot that it should go in, is not currently taken, place this grenade obj there.
    /// Else do nothing with it.
    /// </summary>
    /// <param name="weaponToPickup"></param>
    private void PickupGrenade(GameObject grenadeToPickup)
    {
        Grenades grenadeInfo = grenadeToPickup.GetComponent<Grenades>();
        if (inventory[grenadeInfo.weaponSlot] == null)
        {
            grenadeToPickup.transform.position = this.transform.position;
            grenadeToPickup.transform.rotation = this.transform.rotation;
            grenadeToPickup.transform.parent = this.transform;
            grenadeToPickup.GetComponent<Rigidbody>().useGravity = false;
            grenadeToPickup.GetComponent<Rigidbody>().isKinematic = true;
            grenadeToPickup.GetComponent<Rigidbody>().detectCollisions = false;
            grenadeToPickup.GetComponent<Collider>().enabled = false;

            inventory[grenadeInfo.weaponSlot] = grenadeToPickup;

            this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
        }
    }
}
