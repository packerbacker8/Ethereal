using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponManager : NetworkBehaviour
{
    public GameObject weaponInventory;
    #region combat keycodes
    [Header("Inventory Keys")]
    public KeyCode primaryWeapon = KeyCode.Alpha1;
    public KeyCode secondaryWeapon = KeyCode.Alpha2;
    public KeyCode knife = KeyCode.Alpha3;
    public KeyCode grenades = KeyCode.Alpha4;
    public KeyCode bomb = KeyCode.Alpha5;
    public KeyCode teamItem = KeyCode.Alpha6;
    public KeyCode quickSwitch = KeyCode.Q;
    public KeyCode throwCurrentWeapon = KeyCode.T;
    #endregion

    private InventoryScript inventory;
    private List<string> weaponsEquipped;
    private List<GameObject> weapons;
    private const string WEAPON_LAYER_NAME = "Weapon";
    private int currentWeaponIndex;
    private int previousWeaponIndex;

    private bool amidstInventoryStuff;

    private List<string> inventorySlotStrings;

    private Weapon currentWeapon;

    // Use this for initialization
    private void Start()
    {
        inventory = weaponInventory.GetComponent<InventoryScript>();
        if(inventory == null)
        {
            Debug.LogError("No InventoryScript found on this player: " + this.name);
            return;
        }
        weaponsEquipped = new List<string>();
        weapons = new List<GameObject>();
        inventorySlotStrings = inventory.GetAllInventorySlotNames();
        SetNonNullWeaponsFromInventory();
        currentWeaponIndex = 0;
        previousWeaponIndex = currentWeaponIndex;
        amidstInventoryStuff = false;
        Debug.Log("Weapon manager start " + this.name);
    }

    private void Update()
    {
        if(weaponsEquipped.Count == 0 || amidstInventoryStuff)
        {
            return;
        }

        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        bool switchToPrimary = Input.GetKeyDown(primaryWeapon);
        bool switchToSecondary = Input.GetKeyDown(secondaryWeapon);
        bool switchToKnife = Input.GetKeyDown(knife);
        bool switchToGrenade = Input.GetKeyDown(grenades);
        bool switchToBomb = Input.GetKeyDown(bomb);
        bool switchToTeamItem = Input.GetKeyDown(teamItem);
        bool quickSwitchItem = Input.GetKeyDown(quickSwitch);
        bool tossCurrentWep = Input.GetKeyDown(throwCurrentWeapon);

        if (tossCurrentWep)
        {
            amidstInventoryStuff = true;
            inventory.RemoveWeapon(weaponsEquipped[currentWeaponIndex]);
        }
        else
        {
            if (mouseScroll > 0)
            {
                currentWeaponIndex = (currentWeaponIndex + 1) % weaponsEquipped.Count;
            }
            else if (mouseScroll < 0)
            {
                currentWeaponIndex = Mathf.Abs((currentWeaponIndex - 1)) % weaponsEquipped.Count;
            }

            if (switchToPrimary)
            {
                currentWeaponIndex = 0;
            }

            if (switchToSecondary && weaponsEquipped.Count > 1)
            {
                currentWeaponIndex = 1;
            }

            if (quickSwitchItem)
            {
                int temp = currentWeaponIndex;
                currentWeaponIndex = previousWeaponIndex;
                previousWeaponIndex = temp;
            }

            if (currentWeaponIndex != previousWeaponIndex)
            {
                previousWeaponIndex = currentWeaponIndex;
                EquipWeapon();
            }
        }
    }

    private void FirstTimeSetup()
    {
        SetupWeapons();
    }

    /// <summary>
    /// Any time the weapon is changed set this up.
    /// </summary>
    private void SetupWeapons()
    {
        if(weapons.Count == 0)
        {
            Debug.Log("No weapons");
            currentWeapon = null;
            this.SendMessage("WeaponChanged");
            return;
        }
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        weapons[currentWeaponIndex].SetActive(true);

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        currentWeapon.weaponGraphics.SetActive(true);
        if (isLocalPlayer && currentWeapon.weaponGraphics.layer != LayerMask.NameToLayer(WEAPON_LAYER_NAME))
        {
            Util.SetLayerRecusively(currentWeapon.weaponGraphics, LayerMask.NameToLayer(WEAPON_LAYER_NAME));
        }

        this.SendMessage("WeaponChanged");
    }

    /// <summary>
    /// A method to set the inventory items that the player currently has so we know which are null
    /// and which are not. Also sets the current weapon index to the highest priority one. I.e.
    /// primary is highest, then secondary, etc, down to the objective bomb. Team items will not be
    /// in this list.
    /// </summary>
    [Client]
    public void SetNonNullWeaponsFromInventory()
    {
        if (isLocalPlayer)
        {
            weaponsEquipped.Clear();
            weapons.Clear();
            foreach (string slotName in inventorySlotStrings)
            {
                if (inventory.GetWeapon(slotName) != null)
                {
                    weaponsEquipped.Add(slotName);
                    weapons.Add(inventory.GetWeapon(slotName));
                }
            }
            currentWeaponIndex = 0;
            SetupWeapons();
            amidstInventoryStuff = false;
        }
    }

    [Client]
    private void EquipWeapon()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CmdOnEquipWeapon(currentWeaponIndex, this.name);
    }

    [Command]
    private void CmdOnEquipWeapon(int idx, string idOfPlayerChangingGuns)
    {
        RpcDoEquipWeapon(idx, idOfPlayerChangingGuns);
    }

    [ClientRpc]
    private void RpcDoEquipWeapon(int idx, string idOfPlayerChangingGuns)
    {
        currentWeaponIndex = idx;
        SetupWeapons();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    /// <summary>
    /// This returns true if there are changes occuring in the inventory script that 
    /// have not yet propogated to the weapon manager.
    /// </summary>
    /// <returns></returns>
    public bool GetInInventoryChanging()
    {
        return amidstInventoryStuff;
    }
}
