using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    private bool isDead = false;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    private Collider col;
    private GameObject playerUIObj;
    private PlayerUIScript playerUI;

    /// <summary>
    /// Initializes disable on death arrays and was enabled arrays
    /// </summary>
    public void Setup()
    {
        col = this.GetComponent<Collider>();
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        playerUIObj = this.GetComponent<PlayerSetup>().GetPlayerUI();
        if(playerUIObj != null)
        {
            playerUI = playerUIObj.GetComponent<PlayerUIScript>();
        }
        else
        {
            playerUI = null;
        }
        SetDefaults();
    }

    /// <summary>
    /// Give the player damage amount and subtract it from the current health.
    /// </summary>
    /// <param name="amount"></param>
    [ClientRpc]
    public void RpcTakeDamage(int amount)
    {
        if (!Dead)
        {
            currentHealth -= amount;            
            if(currentHealth <= 0)
            {
                currentHealth = 0;
                KillPlayer();
            }
            if (playerUI != null)
            {
                playerUI.SetHealth((float)currentHealth / maxHealth);
            }
        }
    }

    /// <summary>
    /// Make player dead.
    /// </summary>
    public void KillPlayer()
    {
        isDead = true;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        if (col != null)
        {
            col.enabled = false;
        }

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
        SetDefaults();
        Transform respawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = respawnPoint.position;
    }

    /// <summary>
    /// Reset player values to their default startng values.
    /// A part of respawning.
    /// </summary>
    public void SetDefaults()
    {
        currentHealth = maxHealth;
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
    }
}
