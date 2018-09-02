using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Personal Weapon Prefabs")]
    public GameObject ak47Obj;
    public GameObject m416Obj;
    public GameObject m16a4Obj;
    public GameObject m60Obj;
    public GameObject m249Obj;
    public GameObject ump45Obj;
    public GameObject vectorObj;
    public GameObject uziObj;
    public GameObject mp9Obj;
    public GameObject spas12Obj;
    public GameObject overunderObj;
    public GameObject mag7Obj;
    public GameObject ksgObj;
    public GameObject cal50Obj;
    public GameObject m24Obj;
    public GameObject glockObj;
    public GameObject m9Obj;
    public GameObject desertEagleObj;
    public GameObject rexObj;
    [Header("Grenade Prefabs")]
    public GameObject fragObj;
    public GameObject flashObj;
    public GameObject flameObj;
    public GameObject smokeObj;

    [Header("Team Item Prefabs")]
    public GameObject orbitalLaserStrikeObj;
    public GameObject teamProtectionDomeObj;

    public static bool IsInEconomyMenu;


    [SerializeField]
    private RectTransform healthFill;

    [SerializeField]
    private GameObject pauseMenuObj;

    private GameObject player;
    private InventoryScript playerInventory;

    public void SetPlayer(GameObject player)
    {
        this.player = player;
        playerInventory = this.player.GetComponentInChildren<InventoryScript>();
    }

    // Use this for initialization
    void Start()
    {
        healthFill.localScale = Vector3.one;
        PauseMenu.isPaused = false;
        IsInEconomyMenu = false;
        ToggleOffAllMenus();
        SetCursorCombatMode();
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
                else if(personalSellObj.activeInHierarchy || personalBuyObj.activeInHierarchy || teamBuyObj.activeInHierarchy)
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

    public void BuyAk47()
    {
        GameObject ak47 = Instantiate(ak47Obj);
        ak47.name = ak47Obj.name;
        if(!playerInventory.SetWeapon(ak47.GetComponent<Weapon>().weaponSlot, ak47))
        {
            Debug.LogError(ak47.name + " not successfully added");
        }
    }

    public void BuyUmp45()
    {
        GameObject ump45 = Instantiate(ump45Obj);
        ump45.name = ump45Obj.name;
        if(!playerInventory.SetWeapon(ump45.GetComponent<Weapon>().weaponSlot, ump45))
        {
            Debug.LogError(ump45.name + " not successfully added");
        }
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
}
