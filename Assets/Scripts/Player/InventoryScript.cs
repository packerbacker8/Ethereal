using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public bool autoPickup = true;
    public float pickupRange = 10f;
    public KeyCode interact = KeyCode.E;


    [Header("Inventory Slot Keys")]
    public const string PRIMARY = "primary";
    public const string SECONDARY = "secondary";
    public const string KNIFE = "knife";
    public const string GRENADES = "grendades";
    public const string BOMB = "bomb";

    public const string WEAPON_LAYER = "Weapon";
    public const string WORLD_OBJECT_LAYER = "WorldObject";

    public const int GRENADE_COUNT = 4;

    private GameObject playerCam;
    private InventoryUIScript inventoryUi;

    private Dictionary<string, GameObject[]> inventory;

    private void Start()
    {
        playerCam = this.transform.parent.gameObject;
    }

    public void SetupInventory(GameObject playerUIObj)
    {
        inventory = new Dictionary<string, GameObject[]>() {
            { PRIMARY, new GameObject[] {null} }, 
            { SECONDARY, new GameObject[] {null}},
            { KNIFE, new GameObject[] {null}},
            { GRENADES, new GameObject[] {null, null, null, null}},
            { BOMB, new GameObject[] {null}}
        };

        //see if there are weapons currently parented to the inventory management item
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;

            if(child.GetComponent<Weapon>() != null)
            {
                child.GetComponent<Rigidbody>().useGravity = false;
                child.GetComponent<Rigidbody>().isKinematic = true;
                child.GetComponent<Rigidbody>().detectCollisions = false;
                child.GetComponent<Collider>().enabled = false;
                inventory[child.GetComponent<Weapon>().weaponSlot][0] = child; //always position 0 because there is only one item for these slots 

            }
            else if(child.GetComponent<Grenades>() != null)
            {
                child.GetComponent<Rigidbody>().useGravity = false;
                child.GetComponent<Rigidbody>().isKinematic = true;
                child.GetComponent<Rigidbody>().detectCollisions = false;
                child.GetComponent<Collider>().enabled = false;
                //loop over four game object spots for grenades
                for (int j = 0; j < 4; j++)
                {
                    if(inventory[child.GetComponent<Grenades>().weaponSlot][j] == null)
                    {
                        inventory[child.GetComponent<Grenades>().weaponSlot][j] = child;
                        break;
                    }
                }
            }
        }
        if(playerUIObj == null)
        {
            Debug.LogError("Player UI was null in the set up of the inventory script.");
        }
        else
        {
            inventoryUi = playerUIObj.GetComponent<InventoryUIScript>();
        }

    }

    private void Update()
    {
        if(inventory == null)
        {
            return;
        }

        bool interactPressed = Input.GetKeyDown(interact);

        if (interactPressed)
        {
            AttemptToPickup();
        }
    }

    private void AttemptToPickup()
    {
        RaycastHit hit;
        Debug.DrawLine(playerCam.transform.position, playerCam.transform.position + playerCam.transform.forward * pickupRange, Color.blue, 5f);
        int mask = 1 << LayerMask.NameToLayer(WORLD_OBJECT_LAYER);
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, pickupRange, mask))
        {
            GameObject hitObj = hit.transform.gameObject;
            if(hitObj.GetComponent<Weapon>() != null)
            {
                SetWeapon(hitObj.GetComponent<Weapon>().weaponSlot, hitObj);
            }
            else if(hitObj.GetComponent<Grenades>() != null)
            {
                int nextAvailIndex = GetNextAvailGrenadeSpot();
                if(nextAvailIndex == -1)
                {
                    return;
                }
                SetWeapon(hitObj.GetComponent<Grenades>().weaponSlot, hitObj, nextAvailIndex);
            }
        }
    }

    public List<string> GetAllInventorySlotNames()
    {
        return new List<string>() { PRIMARY, SECONDARY, KNIFE, GRENADES, BOMB };
    }

    public Dictionary<string, GameObject[]> GetInventory()
    {
        return inventory;
    }

    public GameObject GetWeapon(string slot, int grenadeIndex = 0)
    {
        //inventoryUi.InventoryActionPerformed();
        switch (slot)
        {
            case PRIMARY:
                return GetPrimaryWeapon();
            case SECONDARY:
                return GetSecondaryWeapon();
            case KNIFE:
                return GetKnife();
            case GRENADES:
                return GetGrenade(grenadeIndex % GRENADE_COUNT);
            case BOMB:
                return GetObjectiveBomb();
            default:
                Debug.LogError("This is not a recognized inventory slot: " + slot);
                return null;
        }
    }

    public GameObject GetPrimaryWeapon()
    {
        return inventory[PRIMARY][0];
    }

    public GameObject GetSecondaryWeapon()
    {
        return inventory[SECONDARY][0];
    }

    public GameObject GetKnife()
    {
        return inventory[KNIFE][0];
    }

    public GameObject GetGrenade(int index)
    {
        return inventory[GRENADES][index];
    }

    public GameObject[] GetGrenades()
    {
        return inventory[GRENADES];
    }

    public GameObject GetObjectiveBomb()
    {
        return inventory[BOMB][0];
    }

    public void RemoveWeapon(string slot, int grenadeIndex = 0)
    {
        GameObject wepToRemove;
        switch (slot)
        {
            case PRIMARY:
                wepToRemove = inventory[PRIMARY][0];
                inventory[PRIMARY][0] = null;
                break;
            case SECONDARY:
                wepToRemove = inventory[SECONDARY][0];
                inventory[SECONDARY][0] = null;
                break;
            case KNIFE:
                wepToRemove = inventory[KNIFE][0];
                inventory[KNIFE][0] = null;
                break;
            case GRENADES:
                wepToRemove = inventory[GRENADES][grenadeIndex % GRENADE_COUNT];
                inventory[GRENADES][grenadeIndex % GRENADE_COUNT] = null;
                break;
            case BOMB:
                wepToRemove = inventory[BOMB][0];
                inventory[BOMB][0] = null;
                break;
            default:
                Debug.LogError("This is not a recognized inventory slot: " + slot);
                return;
        }
        wepToRemove.transform.parent = null;
        Util.SetLayerRecusively(wepToRemove, LayerMask.NameToLayer(WORLD_OBJECT_LAYER));
        wepToRemove.transform.position = wepToRemove.transform.position + wepToRemove.transform.forward * (this.GetComponent<SphereCollider>().radius * 2.5f);
        wepToRemove.transform.position = new Vector3(wepToRemove.transform.position.x, this.transform.position.y, wepToRemove.transform.position.z);
        wepToRemove.GetComponent<Rigidbody>().useGravity = true;
        wepToRemove.GetComponent<Rigidbody>().isKinematic = false;
        wepToRemove.GetComponent<Rigidbody>().detectCollisions = true;
        wepToRemove.GetComponent<Rigidbody>().AddForce(wepToRemove.transform.forward * 1000f);
        wepToRemove.GetComponent<Rigidbody>().AddTorque(wepToRemove.transform.forward * 1000f);
        wepToRemove.GetComponent<Collider>().enabled = true;
        this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
        inventoryUi.InventoryActionPerformed();
    }

    public void ClearInventory()
    {
        inventory = new Dictionary<string, GameObject[]>() {
            { PRIMARY, new GameObject[] {null} },
            { SECONDARY, new GameObject[] {null}},
            { KNIFE, new GameObject[] {null}},
            { GRENADES, new GameObject[] {null, null, null, null}},
            { BOMB, new GameObject[] {null}}
        };
    }

    public bool SetWeapon(string slot, GameObject wep, int grenadeIndex = 0)
    {
        bool success;
        switch (slot)
        {
            case PRIMARY:
                success = SetPrimaryWeapon(wep);
                break;
            case SECONDARY:
                success = SetSecondaryWeapon(wep);
                break;
            case KNIFE:
                success = SetKnifeWeapon(wep);
                break;
            case GRENADES:
                success = SetGrenade(wep, grenadeIndex % GRENADE_COUNT);
                break;
            case BOMB:
                success = SetObjectiveBomb(wep);
                break;
            default:
                Debug.LogError("This is not a recognized inventory slot: " + slot);
                return false;
        }
        if (success)
        {
            PrepareWeaponForAddToPlayer(wep);
            this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
            inventoryUi.InventoryActionPerformed();
        }
        return success;
    }

    private bool SetPrimaryWeapon(GameObject wep)
    {
        if(wep == null || wep.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if(inventory[PRIMARY][0] != null)
        {
            RemoveWeapon(PRIMARY);
        }
        inventory[PRIMARY][0] = wep;
        return true;
    }

    private bool SetSecondaryWeapon(GameObject wep)
    {
        if (wep == null || wep.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if (inventory[SECONDARY][0] != null)
        {
            RemoveWeapon(SECONDARY);
        }
        inventory[SECONDARY][0] = wep;
        return true;
    }

    private bool SetKnifeWeapon(GameObject wep)
    {
        if (wep == null || wep.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if (inventory[KNIFE][0] != null)
        {
            RemoveWeapon(KNIFE);
        }
        inventory[KNIFE][0] = wep;
        return true;
    }

    private bool SetGrenade(GameObject grenade, int grenadeIndex)
    {
        if (grenade == null || grenade.GetComponent<Grenades>() == null)
        {
            return false;
        }
        if (AllGrenadeSpotsTaken())
        {
            return false;
        }
        if (inventory[GRENADES][grenadeIndex] != null)
        {
            RemoveWeapon(GRENADES);
        }
        inventory[GRENADES][grenadeIndex] = grenade;
        return true;
    }

    private int GetNextAvailGrenadeSpot()
    {
        for (int i = 0; i < GRENADE_COUNT; i++)
        {
            if(inventory[GRENADES][i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    private bool AllGrenadeSpotsTaken()
    {
        foreach(GameObject grenadeSpot in inventory[GRENADES])
        {
            if(grenadeSpot == null)
            {
                return false;
            }
        }
        return true;
    }

    private bool SetObjectiveBomb(GameObject bomb)
    {
        if (bomb == null || bomb.GetComponent<Weapon>() == null)
        {
            return false;
        }
        if (inventory[BOMB][0] != null)
        {
            RemoveWeapon(BOMB);
        }
        inventory[BOMB][0] = bomb;
        return true;
    }

    private void PrepareWeaponForAddToPlayer(GameObject wepToPrep)
    {
        wepToPrep.transform.position = this.transform.position;
        wepToPrep.transform.rotation = this.transform.rotation;
        wepToPrep.transform.parent = this.transform;
        wepToPrep.GetComponent<Rigidbody>().useGravity = false;
        wepToPrep.GetComponent<Rigidbody>().isKinematic = true;
        wepToPrep.GetComponent<Rigidbody>().detectCollisions = false;
        wepToPrep.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// Collider that is being used to pickup weapons.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        if (autoPickup)
        {
            if (col.gameObject.GetComponentInChildren<Weapon>() != null)
            {
                PickupWeapon(col.gameObject);
            }
            else if (col.gameObject.GetComponentInChildren<Grenades>() != null)
            {
                PickupGrenade(col.gameObject);
            }
        }
    }


    /// <summary>
    /// Collider that is being used to pickup weapons.
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerStay(Collider col)
    {
        if (autoPickup)
        {
            if (col.gameObject.GetComponentInChildren<Weapon>() != null)
            {
                PickupWeapon(col.gameObject);
            }
            else if (col.gameObject.GetComponentInChildren<Grenades>() != null)
            {
                PickupGrenade(col.gameObject);
            }
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
        if(inventory[wepInfo.weaponSlot][0] == null)
        {
            //weaponToPickup.transform.position = this.transform.position;
            //weaponToPickup.transform.rotation = this.transform.rotation;
            //weaponToPickup.transform.parent = this.transform;
            //weaponToPickup.GetComponent<Rigidbody>().useGravity = false;
            //weaponToPickup.GetComponent<Rigidbody>().isKinematic = true;
            //weaponToPickup.GetComponent<Rigidbody>().detectCollisions = false;
            //weaponToPickup.GetComponent<Collider>().enabled = false;\

            PrepareWeaponForAddToPlayer(weaponToPickup);

            inventory[wepInfo.weaponSlot][0] = weaponToPickup;

            this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
            inventoryUi.InventoryActionPerformed();
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
        for (int i = 0; i < GRENADE_COUNT; i++)
        {
            if (inventory[grenadeInfo.weaponSlot][i] == null)
            {
                PrepareWeaponForAddToPlayer(grenadeToPickup);

                inventory[grenadeInfo.weaponSlot][i] = grenadeToPickup;

                this.SendMessageUpwards("SetNonNullWeaponsFromInventory", SendMessageOptions.DontRequireReceiver);
                inventoryUi.InventoryActionPerformed();
            }
        }
    }
}
