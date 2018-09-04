using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIScript : MonoBehaviour
{
    public GameObject inventoryAndMoneyPanel;
    public GameObject overallInventoryUI;
    public GameObject primaryWepImageObj;
    public GameObject secondaryWepImageObj;
    public GameObject knifeImageObj;
    public GameObject grenadePanelObj;
    public GameObject objectiveBombImageObj;
    public Text playerMoneyText;

    public float inventoryDisappearTime = 1.5f;
    public float timeBeforeStartDisappearing = 2.0f;
    
    private GameObject player;
    private InventoryScript playerInventory;
    private PlayerManager playerManager;
    private List<string> inventorySlotNames;

    private float uiOpacity = 1.0f;
    private float disappearTimer;
    private float startDisappearingTimer;

    // Update is called once per frame
    private void Update()
    {
        if(player == null)
        {
            return;
        }
        if(uiOpacity > 0)
        {
            startDisappearingTimer -= Time.deltaTime;
            if (startDisappearingTimer <= 0)
            {
                startDisappearingTimer = 0;
                disappearTimer -= Time.deltaTime;
                if(disappearTimer <= 0)
                {
                    uiOpacity = 0;
                    inventoryAndMoneyPanel.SetActive(false);
                }
                else
                {
                    uiOpacity = disappearTimer / inventoryDisappearTime;
                    SetUIOpacity();
                }
            }
        }
    }

    /// <summary>
    /// Function called at the creation of the player UI to let the script know which player it is for.
    /// </summary>
    /// <param name="player"></param>
    public void SetPlayerTarget(GameObject player)
    {
        this.player = player;
        playerInventory = this.player.GetComponentInChildren<InventoryScript>();
        playerManager = this.player.GetComponentInChildren<PlayerManager>();
        inventorySlotNames = playerInventory.GetAllInventorySlotNames();
    }

    /// <summary>
    /// Function called anytime the full inventory and money UI need to be displayed for the player.
    /// </summary>
    public void InventoryActionPerformed()
    {
        inventoryAndMoneyPanel.SetActive(true);
        overallInventoryUI.SetActive(true);
        disappearTimer = inventoryDisappearTime;
        startDisappearingTimer = timeBeforeStartDisappearing;
        uiOpacity = 1.0f;
        playerMoneyText.text = "$" + playerManager.GetCurrentPlayerMoney();
        foreach (string slot in inventorySlotNames)
        {
            if (slot == InventoryScript.GRENADES)
            {
                GameObject[] grenades = playerInventory.GetGrenades();
                for (int i = 0; i < InventoryScript.GRENADE_COUNT; i++)
                {
                    if(grenades[i] == null)
                    {
                        grenadePanelObj.transform.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        Grenades grenade = grenades[i].GetComponent<Grenades>();
                        grenadePanelObj.transform.GetChild(i).gameObject.SetActive(true);
                        grenadePanelObj.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = grenade.uiIcon;
                        grenadePanelObj.transform.GetChild(i).gameObject.GetComponentInChildren<Text>().text = grenade.weaponName;
                    }
                }
            }
            else
            {
                GameObject wepToGrabObj = playerInventory.GetWeapon(slot);
                if (slot == InventoryScript.PRIMARY)
                {
                    //there is no weapon in that slot so turn it off
                    if (wepToGrabObj == null)
                    {
                        primaryWepImageObj.SetActive(false);
                    }
                    else
                    {
                        Weapon wepToGrab = wepToGrabObj.GetComponent<Weapon>();
                        primaryWepImageObj.SetActive(true);
                        primaryWepImageObj.GetComponent<Image>().sprite = wepToGrab.uiIcon;
                        primaryWepImageObj.GetComponentInChildren<Text>().text = wepToGrab.weaponName;
                    }
                }
                else if (slot == InventoryScript.SECONDARY)
                {
                    //there is no weapon in that slot so turn it off
                    if (wepToGrabObj == null)
                    {
                        secondaryWepImageObj.SetActive(false);
                    }
                    else
                    {
                        Weapon wepToGrab = wepToGrabObj.GetComponent<Weapon>();
                        secondaryWepImageObj.SetActive(true);
                        secondaryWepImageObj.GetComponent<Image>().sprite = wepToGrab.uiIcon;
                        secondaryWepImageObj.GetComponentInChildren<Text>().text = wepToGrab.weaponName;
                    }
                }
                else if (slot == InventoryScript.KNIFE)
                {
                    //there is no weapon in that slot so turn it off
                    if (wepToGrabObj == null)
                    {
                        knifeImageObj.SetActive(false);
                    }
                    else
                    {
                        Weapon wepToGrab = wepToGrabObj.GetComponent<Weapon>();
                        knifeImageObj.SetActive(true);
                        knifeImageObj.GetComponent<Image>().sprite = wepToGrab.uiIcon;
                        knifeImageObj.GetComponentInChildren<Text>().text = wepToGrab.weaponName;
                    }
                }
                else if (slot == InventoryScript.BOMB)
                {
                    //there is no weapon in that slot so turn it off
                    if (wepToGrabObj == null)
                    {
                        objectiveBombImageObj.SetActive(false);
                    }
                    else
                    {
                        Weapon wepToGrab = wepToGrabObj.GetComponent<Weapon>();
                        objectiveBombImageObj.SetActive(true);
                        objectiveBombImageObj.GetComponent<Image>().sprite = wepToGrab.uiIcon;
                        objectiveBombImageObj.GetComponentInChildren<Text>().text = wepToGrab.weaponName;
                    }
                }
            }
        }
        SetUIOpacity();
    }

    /// <summary>
    /// Function used to update and show the text representing the player's 
    /// current money without displaying the rest of the inventory.
    /// </summary>
    public void UpdateAndShowMoneyText()
    {
        inventoryAndMoneyPanel.SetActive(true);
        overallInventoryUI.SetActive(false);
        disappearTimer = inventoryDisappearTime;
        startDisappearingTimer = timeBeforeStartDisappearing;
        uiOpacity = 1.0f;
        playerMoneyText.text = "$" + playerManager.GetCurrentPlayerMoney();
        playerMoneyText.color = new Color(1, 1, 1, uiOpacity);
    }

    /// <summary>
    /// Set opacity of all image objects on the inventory UI so that
    /// the images may fade out over a set amount of time.
    /// </summary>
    private void SetUIOpacity()
    {
        playerMoneyText.color = new Color(1, 1, 1, uiOpacity);
        if (primaryWepImageObj.activeInHierarchy)
        {
            primaryWepImageObj.GetComponent<Image>().color = new Color(1, 1, 1, uiOpacity);
        }
        if (secondaryWepImageObj.activeInHierarchy)
        {
            secondaryWepImageObj.GetComponent<Image>().color = new Color(1, 1, 1, uiOpacity);
        }
        if (knifeImageObj.activeInHierarchy)
        {
            knifeImageObj.GetComponent<Image>().color = new Color(1, 1, 1, uiOpacity);
        }
        if (grenadePanelObj.activeInHierarchy)
        {
            for (int i = 0; i < grenadePanelObj.transform.childCount; i++)
            {
                grenadePanelObj.transform.GetChild(i).gameObject.GetComponent<Image>().color = new Color(1, 1, 1, uiOpacity);
            }
        }
        if (objectiveBombImageObj.activeInHierarchy)
        {
            objectiveBombImageObj.GetComponent<Image>().color = new Color(1, 1, 1, uiOpacity);
        }
    }
}
