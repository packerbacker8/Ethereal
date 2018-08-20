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
    #endregion

    private List<GameObject> weapons;
    private HashSet<string> weaponsEquipped;
    private const string WEAPON_LAYER_NAME = "Weapon";
    private int currentWeaponIndex = 0;
    private int previousWeaponIndex;

    private Weapon currentWeapon;

    // Use this for initialization
    private void Start()
    {
        weapons = new List<GameObject>();
        weaponsEquipped = new HashSet<string>();
        for (int i = 0; i < weaponInventory.transform.childCount; i++)
        {
            weapons.Add(weaponInventory.transform.GetChild(i).gameObject);
            weaponsEquipped.Add(weaponInventory.transform.GetChild(i).name);
        }
        currentWeaponIndex = 0;
        previousWeaponIndex = currentWeaponIndex;
        //EquipWeapon();
        //CmdOnEquipWeapon(currentWeaponIndex, this.name);
        FirstTimeSetup();
        Debug.Log("Weapon manager start " + this.name);
    }

    private void Update()
    {
        //TODO: update weapons across players
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        bool switchToPrimary = Input.GetKeyDown(primaryWeapon);
        bool switchToSecondary = Input.GetKeyDown(secondaryWeapon);
        bool switchToKnife = Input.GetKeyDown(knife);
        bool switchToGrenade = Input.GetKeyDown(grenades);
        bool switchToBomb = Input.GetKeyDown(bomb);
        bool switchToTeamItem = Input.GetKeyDown(teamItem);
        bool quickSwitchItem = Input.GetKeyDown(quickSwitch);


        if (mouseScroll > 0)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
            Debug.Log(currentWeaponIndex);
        }
        else if (mouseScroll < 0)
        {
            currentWeaponIndex = Mathf.Abs((currentWeaponIndex - 1)) % weapons.Count;
        }

        if (switchToPrimary)
        {
            currentWeaponIndex = 0;
        }

        if (switchToSecondary)
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

    private void FirstTimeSetup()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        weapons[currentWeaponIndex].SetActive(true);

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        currentWeapon.weaponGraphics.SetActive(true);
        if (isLocalPlayer)
        {
            Util.SetLayerRecusively(currentWeapon.weaponGraphics, LayerMask.NameToLayer(WEAPON_LAYER_NAME));
        }


        this.gameObject.AddComponent<ObjectPool>();
        this.GetComponent<ObjectPool>().amountPooled = 150;
        this.GetComponent<ObjectPool>().objectToPool = currentWeapon.weaponGraphics.GetComponent<WeaponGraphics>().hitEffectPrefab;
        this.GetComponent<ObjectPool>().SetupPool();

        this.SendMessage("WeaponChanged");
        this.SendMessage("EffectPoolReady");
    }

    [Client]
    private void EquipWeapon()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        Debug.Log("sending " + currentWeaponIndex);
        CmdOnEquipWeapon(currentWeaponIndex, this.name);

        //foreach (GameObject weapon in weapons)
        //{
        //    weapon.SetActive(false);
        //}

        //weapons[currentWeaponIndex].SetActive(true);

        //currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        //currentWeapon.weaponGraphics.SetActive(true);
        //if (isLocalPlayer)
        //{
        //    Util.SetLayerRecusively(currentWeapon.weaponGraphics, LayerMask.NameToLayer(WEAPON_LAYER_NAME));
        //}

        //this.SendMessage("WeaponChanged");
    }

    [Command]
    private void CmdOnEquipWeapon(int idx, string idOfPlayerChangingGuns)
    {
        RpcDoEquipWeapon(idx, idOfPlayerChangingGuns);
    }

    [ClientRpc]
    private void RpcDoEquipWeapon(int idx, string idOfPlayerChangingGuns)
    {
        Debug.Log("Changing weapon for " + idOfPlayerChangingGuns);
        Debug.Log("Receiving " + idx);
        currentWeaponIndex = idx;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        weapons[currentWeaponIndex].SetActive(true);

        currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        currentWeapon.weaponGraphics.SetActive(true);
        if (isLocalPlayer)
        {
            Util.SetLayerRecusively(currentWeapon.weaponGraphics, LayerMask.NameToLayer(WEAPON_LAYER_NAME));
        }

        this.SendMessage("WeaponChanged");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
