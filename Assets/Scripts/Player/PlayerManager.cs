using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerSetup))]
public class PlayerManager : NetworkBehaviour
{
    public int maxHealth = 100;
    public bool Dead
    {
        get
        {
            return isDead;
        }
        protected set
        {
            isDead = value;
        }
    }

    [SyncVar]
    private int currentHealth;
    [SyncVar]
    private int currentMoney;

    [SyncVar]
    private bool isDead = false;
    private bool firstSetup = true;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    private Collider col;
    private GameObject playerUIObj;
    private PlayerUIScript playerUI;

    [SerializeField]
    private GameObject deathEffect; //this will likely just be an animation
    [SerializeField]
    private GameObject spawnEffect;


    /// <summary>
    /// Initializes disable on death arrays and was enabled arrays
    /// </summary>
    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            this.GetComponent<PlayerSetup>().GetPlayerUI().SetActive(true);
            GameManager.instance.SetWorldCameraActive(false);
            CmdBroadCastNewPlayerSetup();
        }
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            col = this.GetComponent<Collider>();
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            playerUIObj = this.GetComponent<PlayerSetup>().GetPlayerUI();
            if (playerUIObj != null)
            {
                playerUI = playerUIObj.GetComponent<PlayerUIScript>();
            }
            else
            {
                playerUI = null;
            }
            firstSetup = false;
        }
        
        SetDefaults();
    }

    /// <summary>
    /// Give the player damage amount and subtract it from the current health.
    /// </summary>
    /// <param name="amount">How much damage to take.</param>
    [ClientRpc]
    public void RpcTakeDamage(int amount, string shooterId, int killVal, int teamVal)
    {
        if (!Dead)
        {
            currentHealth -= amount;            
            if(currentHealth <= 0)
            {
                currentHealth = 0;
                KillPlayer();
                // TODO: fix this
                if (isLocalPlayer)
                {
                    CmdGivePlayerMoney(shooterId, killVal, teamVal);
                }                
            }
            if (playerUI != null)
            {
                playerUI.SetHealth((float)currentHealth / maxHealth);
            }
        }
    }

    [Command]
    public void CmdGivePlayerMoney(string shooterId, int killVal, int teamVal)
    {
        PlayerManager shootingPlayer = GameManager.GetPlayer(shooterId);
        shootingPlayer.RpcGivePlayerMoney(killVal, teamVal);
    }

    [ClientRpc]
    public void RpcGivePlayerMoney(int killVal, int teamVal)
    {
        AdjustPlayerMoney(killVal);
        //do something with team money here
    }

    /// <summary>
    /// Make player dead.
    /// </summary>
    public void KillPlayer()
    {
        isDead = true;
        if (isLocalPlayer)
        {
            this.GetComponent<PlayerController>().playerCam.gameObject.SetActive(false);
            this.GetComponent<PlayerSetup>().GetPlayerUI().SetActive(false);
            GameManager.instance.SetWorldCameraActive(true);
            this.GetComponent<Rigidbody>().useGravity = false;
        }
        //disable components to prevent player control
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //disable collider
        if (col != null)
        {
            col.enabled = false;
        }

        //spawn death effect
        //GameObject death = Instantiate(deathEffect, this.transform.position, Quaternion.identity);
        //Destroy(death, 3f);

        Debug.Log(this.transform.name + " has died.");
        StartCoroutine(Respawn());
    }

    /// <summary>
    /// Respawn the player after 3 seconds.
    /// This will be changed for when the round is over that is when the respawn will happen.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        //get position to spawn at
        Transform respawnPoint = NetworkManager.singleton.GetStartPosition();
        this.transform.position = respawnPoint.position;
        this.transform.rotation = respawnPoint.rotation;
        //wait for position to spawn at to be sent across network before spawning
        yield return new WaitForSeconds(0.1f);

        SetupPlayer();
    }

    /// <summary>
    /// Reset player values to their default startng values.
    /// A part of respawning.
    /// </summary>
    public void SetDefaults()
    {
        //might not need this if statement
        if (isLocalPlayer)
        {
            this.GetComponent<PlayerController>().playerCam.gameObject.SetActive(true);
            this.GetComponent<Rigidbody>().useGravity = true;
        }
        currentHealth = maxHealth;
        SetCurrentPlayerMoney(GameManager.instance.matchSettings.startPersonalMoney);
        if (playerUI != null)
        {
            playerUI.SetHealth(1f);
        }
        isDead = false;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        if(col != null)
        {
            col.enabled = true;
        }

        GameObject spwnEff = Instantiate(spawnEffect, this.transform.position, Quaternion.identity);
        Destroy(spwnEff, 3f);
    }

    /// <summary>
    /// Returns the current value of the players money.
    /// </summary>
    /// <returns>Integer value of the current money available.</returns>
    public int GetCurrentPlayerMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// Set the player's money to a whole new amount, a direct equal operation.
    /// </summary>
    /// <param name="newAmount">The new amount the player's money will be set to.</param>
    public void SetCurrentPlayerMoney(int newAmount)
    {
        currentMoney = newAmount;
        playerUI?.gameObject.SendMessage("PlayerMoneyChanged", SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Adjust the player's money by some amount. Note this is always the player's
    /// current money plus some given value. So to take away money, pass a negative
    /// number.
    /// </summary>
    /// <param name="val">Positive or negative integer representing how much to 
    /// change the player's current money by.</param>
    public void AdjustPlayerMoney(int val)
    {
        //TODO: put cap on money so it can't go above personal limit of this match setting
        currentMoney += val;
        playerUI?.gameObject.SendMessage("PlayerMoneyChanged", SendMessageOptions.DontRequireReceiver);
    }
}
