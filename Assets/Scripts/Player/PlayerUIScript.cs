using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    public KeyCode pauseGameKey = KeyCode.Escape;
    public KeyCode economyMenuKey = KeyCode.B;
    public KeyCode personalBuyMenuKey = KeyCode.N;
    public KeyCode teamBuyMenuKey = KeyCode.M;
    public KeyCode personalSellMenuKey = KeyCode.H;

    [Header("Economy UI Objects")]
    public GameObject economyObj;
    public GameObject personalBuyObj;
    public GameObject personalSellObj;
    public GameObject teamBuyObj;
    public GameObject assaultRifleObj;
    public GameObject hmgObj;
    public GameObject smgObj;
    public GameObject shotgunObj;
    public GameObject sniperObj;
    public GameObject handgunObj;
    public GameObject grenadeObj;

    public static bool IsInEconomyMenu;


    [SerializeField]
    private RectTransform healthFill;

    [SerializeField]
    private GameObject pauseMenuObj;

    private GameObject player;
    private InventoryScript playerInventory;
    private PlayerManager playerManager;

    private Dictionary<string, GameObject> personalWeaponPrefabs;
    private Dictionary<string, GameObject> grenadePrefabs;
    private Dictionary<string, GameObject> teamItemPrefabs;

    private List<GameObject> listPersonalWepBuyMenuObjs;

    private int currentMoney;

    public void SetUpPlayerUIScript(GameObject player)
    {
        this.player = player;
        playerInventory = this.player.GetComponentInChildren<InventoryScript>();
        playerManager = this.player.GetComponentInChildren<PlayerManager>();

        personalWeaponPrefabs = new Dictionary<string, GameObject>();
        grenadePrefabs = new Dictionary<string, GameObject>();
        teamItemPrefabs = new Dictionary<string, GameObject>();

        Object[] tempWeps = Resources.LoadAll("PersonalWeapons");
        //Object[] tempGrenades = Resources.LoadAll("Grenades");
        //Object[] tempTeam = Resources.LoadAll("TeamItems");

        foreach (Object wep in tempWeps)
        {
            GameObject wepGO = wep as GameObject;
            string wepName = wepGO.GetComponent<Weapon>().weaponName;
            personalWeaponPrefabs.Add(wepName, wepGO);
        }

        listPersonalWepBuyMenuObjs = new List<GameObject>
        {
            assaultRifleObj,
            hmgObj,
            smgObj,
            shotgunObj,
            sniperObj,
            handgunObj
        };


        healthFill.localScale = Vector3.one;
        PauseMenu.isPaused = false;
        IsInEconomyMenu = false;
        ToggleOffAllMenus();
        SetCursorCombatMode();

        PlayerMoneyChanged();
    }

    private void Update()
    {
        bool pause = Input.GetKeyDown(pauseGameKey);
        bool economyPressed = Input.GetKeyDown(economyMenuKey);
        bool personalBuyPressed = Input.GetKeyDown(personalBuyMenuKey);
        bool personalSellPressed = Input.GetKeyDown(personalSellMenuKey);
        bool teamBuyPressed = Input.GetKeyDown(teamBuyMenuKey);
        IsInEconomyMenu = !AllEconomyMenusOff();

        if (pause)
        {
            //not in a menu, bring up pause menu options
            if (AllEconomyMenusOff())
            {
                TogglePauseMenu();
            }
            else //in some menu, back out to appropriate state
            {
                // if in buy menu of one of the weapon sections
                if (InSectionBuyMenu())
                {
                    assaultRifleObj.SetActive(false);
                    hmgObj.SetActive(false);
                    smgObj.SetActive(false);
                    shotgunObj.SetActive(false);
                    sniperObj.SetActive(false);
                    handgunObj.SetActive(false);
                    grenadeObj.SetActive(false);
                    personalBuyObj.SetActive(true);
                }
                // in one deeper from economy menu
                else if (personalSellObj.activeInHierarchy || personalBuyObj.activeInHierarchy || teamBuyObj.activeInHierarchy)
                {
                    personalBuyObj.SetActive(false);
                    personalSellObj.SetActive(false);
                    teamBuyObj.SetActive(false);
                    economyObj.SetActive(true);
                }
                else // in economy menu top level
                {
                    economyObj.SetActive(false);
                    SetCursorCombatMode();
                }
            }
        }
        else if (economyPressed)
        {
            ToggleEconomyMenu();
        }
        else if (personalBuyPressed)
        {
            TogglePersonalBuyMenu();
        }
        else if (personalSellPressed)
        {
            TogglePersonalSellMenu();
        }
        else if (teamBuyPressed)
        {
            ToggleTeamBuyMenu();
        }
    }

    /// <summary>
    /// Notify the UI that the amount of money available to the player has changed and to update it.
    /// Also updates buttons of menus to the correct interactable or not state.
    /// </summary>
    public void PlayerMoneyChanged()
    {
        currentMoney = playerManager.GetCurrentPlayerMoney();
        SetInteractableStateOfBuyButtons();
    }

    public void SetHealth(float amount)
    {
        healthFill.localScale = new Vector3(amount, 1, 1);
    }

    public void OpenPersonalBuyMenu()
    {
        economyObj.SetActive(false);
        personalBuyObj.SetActive(true);
    }

    public void OpenPersonalSellMenu()
    {
        economyObj.SetActive(false);
        personalSellObj.SetActive(true);
        SetInteractableStateOfSellButtons();
    }

    public void OpenTeamBuyMenu()
    {
        economyObj.SetActive(false);
        teamBuyObj.SetActive(true);
    }

    public void OpenAssaultRifleMenu()
    {
        personalBuyObj.SetActive(false);
        assaultRifleObj.SetActive(true);
    }

    public void OpenHMGMenu()
    {
        personalBuyObj.SetActive(false);
        hmgObj.SetActive(true);
    }

    public void OpenSMGMenu()
    {
        personalBuyObj.SetActive(false);
        smgObj.SetActive(true);
    }

    public void OpenShotgunMenu()
    {
        personalBuyObj.SetActive(false);
        shotgunObj.SetActive(true);
    }

    public void OpenSniperMenu()
    {
        personalBuyObj.SetActive(false);
        sniperObj.SetActive(true);
    }

    public void OpenHandgunMenu()
    {
        personalBuyObj.SetActive(false);
        handgunObj.SetActive(true);
    }

    public void OpenGrenadeMenu()
    {
        personalBuyObj.SetActive(false);
        grenadeObj.SetActive(true);
    }

    /// <summary>
    /// Buy the correct prefabbed weapon based on the weapon name.
    /// </summary>
    /// <param name="wepName">This is the weapon name that corresponds to 
    /// the weapon name assigned to the weapon prefab under its weaponName variable.</param>
    public void BuyPersonalWeapon(string wepName)
    {
        GameObject wepBought = Instantiate(personalWeaponPrefabs[wepName]);
        wepBought.name = personalWeaponPrefabs[wepName].name;
        if (!playerInventory.SetWeapon(wepBought.GetComponent<Weapon>().weaponSlot, wepBought))
        {
            Debug.LogError(wepBought.name + " not successfully added");
        }
        else
        {
            playerManager.AdjustPlayerMoney(wepBought.GetComponent<Weapon>().cost * -1);
        }
    }

    /// <summary>
    /// Sell the equipped primary inventory item of the player, if there is one available.
    /// </summary>
    public void SellPrimaryWeapon()
    {
        GameObject weaponSold = playerInventory.RemoveWeapon(InventoryScript.PRIMARY);
        //nothing was sold as there was no gun there, this shouldn't happen tho as the button should be not activatable
        if(weaponSold == null)
        {
            return;
        }
        Weapon wep = weaponSold.GetComponent<Weapon>();
        playerManager.AdjustPlayerMoney((int)Mathf.Floor(wep.cost * wep.sellBackRatio));
        SetInteractableStateOfSellButtons();
        Destroy(weaponSold);
    }

    /// <summary>
    /// Sell the equipped secondary inventory item of the player, if there is one available.
    /// </summary>
    public void SellSecondaryWeapon()
    {
        GameObject weaponSold = playerInventory.RemoveWeapon(InventoryScript.SECONDARY);
        //nothing was sold as there was no gun there, this shouldn't happen tho as the button should be not activatable
        if (weaponSold == null)
        {
            return;
        }
        Weapon wep = weaponSold.GetComponent<Weapon>();
        playerManager.AdjustPlayerMoney((int)Mathf.Floor(wep.cost * wep.sellBackRatio));
        SetInteractableStateOfSellButtons();
        Destroy(weaponSold);
    }

    //TODO: set up sell grenade button to take to new menu that has you pick which grenade to sell
    //and has a specific script attached to that menu that sends back to this script the value
    //of the index of that grenade to sell
    public void SellAGrenade()
    {

    }

    /// <summary>
    /// Used to toggle off all gameobjects associated under this player ui script.
    /// </summary>
    public void ToggleOffAllMenus()
    {
        pauseMenuObj.SetActive(false);
        economyObj.SetActive(false);
        personalBuyObj.SetActive(false);
        personalSellObj.SetActive(false);
        teamBuyObj.SetActive(false);
        assaultRifleObj.SetActive(false);
        hmgObj.SetActive(false);
        smgObj.SetActive(false);
        shotgunObj.SetActive(false);
        sniperObj.SetActive(false);
        handgunObj.SetActive(false);
        grenadeObj.SetActive(false);
    }

    public void SetCursorCombatMode()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetCursorMenuMode()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private bool AllEconomyMenusOff()
    {
        return !(economyObj.activeInHierarchy || personalBuyObj.activeInHierarchy || personalSellObj.activeInHierarchy || teamBuyObj.activeInHierarchy
            || assaultRifleObj.activeInHierarchy || hmgObj.activeInHierarchy || smgObj.activeInHierarchy || shotgunObj.activeInHierarchy
            || sniperObj.activeInHierarchy || handgunObj.activeInHierarchy || grenadeObj.activeInHierarchy);
    }

    /// <summary>
    /// If we are in any of the sub buy menus for a weapon.
    /// </summary>
    /// <returns></returns>
    private bool InSectionBuyMenu()
    {
        return assaultRifleObj.activeInHierarchy || hmgObj.activeInHierarchy || smgObj.activeInHierarchy || shotgunObj.activeInHierarchy || sniperObj.activeInHierarchy || handgunObj.activeInHierarchy || grenadeObj.activeInHierarchy;
    }

    /// <summary>
    /// Toggle the pause menu from off to on, or vice versa. Turns off all other menus.
    /// </summary>
    private void TogglePauseMenu()
    {
        bool currentStatus = pauseMenuObj.activeInHierarchy;
        ToggleOffAllMenus();
        pauseMenuObj.SetActive(!currentStatus);
        PauseMenu.isPaused = pauseMenuObj.activeInHierarchy;
        if (pauseMenuObj.activeInHierarchy)
        {
            SetCursorMenuMode();
        }
        else
        {
            SetCursorCombatMode();
        }
    }

    /// <summary>
    /// Toggle the economy menu from off to on, or vice versa. Turns off all other menus.
    /// </summary>
    private void ToggleEconomyMenu()
    {
        bool currentStatus = economyObj.activeInHierarchy;
        ToggleOffAllMenus();
        economyObj.SetActive(!currentStatus);
        if (economyObj.activeInHierarchy)
        {
            SetCursorMenuMode();
        }
        else
        {
            SetCursorCombatMode();
        }
    }

    /// <summary>
    /// Toggle the personal buy menu from off to on, or vice versa. Turns off all other menus.
    /// </summary>
    private void TogglePersonalBuyMenu()
    {
        bool currentStatus = personalBuyObj.activeInHierarchy;
        ToggleOffAllMenus();
        personalBuyObj.SetActive(!currentStatus);
        if (personalBuyObj.activeInHierarchy)
        {
            SetCursorMenuMode();
        }
        else
        {
            SetCursorCombatMode();
        }
    }

    /// <summary>
    /// Toggle the personal sell menu from off to on, or vice versa. Turns off all other menus.
    /// </summary>
    private void TogglePersonalSellMenu()
    {
        bool currentStatus = personalSellObj.activeInHierarchy;
        ToggleOffAllMenus();
        personalSellObj.SetActive(!currentStatus);
        if (personalSellObj.activeInHierarchy)
        {
            SetCursorMenuMode();
            SetInteractableStateOfSellButtons();
        }
        else
        {
            SetCursorCombatMode();
        }
    }

    /// <summary>
    /// Toggle the team buy menu from off to on, or vice versa. Turns off all other menus.
    /// </summary>
    private void ToggleTeamBuyMenu()
    {
        bool currentStatus = teamBuyObj.activeInHierarchy;
        ToggleOffAllMenus();
        teamBuyObj.SetActive(!currentStatus);
        if (teamBuyObj.activeInHierarchy)
        {
            SetCursorMenuMode();
        }
        else
        {
            SetCursorCombatMode();
        }
    }

    /// <summary>
    /// Adjusts the interactable state of buy buttons in the menus. If the player
    /// doesn't have enough money, the button will be not interactable.
    /// </summary>
    private void SetInteractableStateOfBuyButtons()
    {
        foreach(GameObject wepBuyMenu in listPersonalWepBuyMenuObjs)
        {
            for (int i = 0; i < wepBuyMenu.transform.childCount; i++)
            {
                GameObject buyButton = wepBuyMenu.transform.GetChild(i).gameObject;
                Button button = buyButton.GetComponent<Button>();
                if (personalWeaponPrefabs.ContainsKey(buyButton.GetComponent<BuyWeaponButtonScript>().weaponName))
                {
                    Weapon wepToCheck = personalWeaponPrefabs[buyButton.GetComponent<BuyWeaponButtonScript>().weaponName].GetComponent<Weapon>();
                    if (wepToCheck.cost > currentMoney)
                    {
                        button.interactable = false;
                    }
                    else
                    {
                        button.interactable = true;
                    }
                }
                else
                {
                    Debug.Log("personalWeaponPrefabs does not contain an entry for: " + buyButton.GetComponent<BuyWeaponButtonScript>().weaponName);
                    button.interactable = false;
                }
            }
        }

        for (int i = 0; i < grenadeObj.transform.childCount; i++)
        {
            GameObject buyButton = grenadeObj.transform.GetChild(i).gameObject;
            Button button = buyButton.GetComponent<Button>();
            if (grenadePrefabs.ContainsKey(buyButton.GetComponent<BuyWeaponButtonScript>().weaponName))
            {
                Grenades grenadeToCheck = grenadePrefabs[buyButton.GetComponent<BuyWeaponButtonScript>().weaponName].GetComponent<Grenades>();
                if (grenadeToCheck.cost > currentMoney)
                {
                    button.interactable = false;
                }
                else
                {
                    button.interactable = true;
                }
            }
            else
            {
                Debug.Log("grenadePrefabs does not contain an entry for: " + buyButton.GetComponent<BuyWeaponButtonScript>().weaponName);
                button.interactable = false;
            }
        }
        /* this will be for the team menu stuff in the future
        for (int i = 0; i < assaultRifleObj.transform.childCount; i++)
        {

        }*/
    }

    /// <summary>
    /// Sets state of the sell buttons to be interactable or not
    /// depending on if the player has an item equipped in that slot.
    /// </summary>
    private void SetInteractableStateOfSellButtons()
    {
        for (int i = 0; i < personalSellObj.transform.childCount; i++)
        {
            GameObject sellSlotMenuObj = personalSellObj.transform.GetChild(i).gameObject;
            Button sellButton = sellSlotMenuObj.GetComponent<Button>();
            //this will be the grenade object
            if(sellSlotMenuObj.transform.childCount > 1)
            {
                sellButton.interactable = false; // TODO: change this once grenades are ready and this menu obj functions as expected
            }
            else
            {
                if(playerInventory.GetWeapon(sellSlotMenuObj.GetComponent<SellWeaponButtonScript>().sellSlotName) == null)
                {
                    sellButton.interactable = false;
                }
                else
                {
                    sellButton.interactable = true;
                }
            }
        }
    }
}
