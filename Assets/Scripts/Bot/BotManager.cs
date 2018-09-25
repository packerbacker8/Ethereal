using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotManager : MonoBehaviour
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

    private int currentHealth;
    private int currentMoney;

    private bool isDead = false;
    private bool firstSetup = true;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;
    private Collider col;

    [SerializeField]
    private GameObject deathEffect; //this will likely just be an animation
    [SerializeField]
    private GameObject spawnEffect;


    /// <summary>
    /// Initializes disable on death arrays and was enabled arrays
    /// </summary>
    public void SetupBot()
    {
        CmdBroadCastNewBotSetup();
    }

    private void CmdBroadCastNewBotSetup()
    {
        RpcSetupBotOnAllClients();
    }

    private void RpcSetupBotOnAllClients()
    {
        if (firstSetup)
        {
            col = this.GetComponent<Collider>();
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            SetCurrentBotMoney(GameManager.instance.matchSettings.startPersonalMoney);
            firstSetup = false;
        }
        SetDefaults();
    }

    /// <summary>
    /// Give the bot damage amount and subtract it from the current health.
    /// </summary>
    /// <param name="amount">How much damage to take.</param>
    public void RpcTakeDamage(int amount, string shooterId, int killVal, int teamVal)
    {
        if (!Dead)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                KillBot();
                // TODO: fix this. need to account for bot or player
                CmdGivePlayerMoney(shooterId, killVal, teamVal);
            }
        }
    }

    public void CmdGivePlayerMoney(string shooterId, int killVal, int teamVal)
    {
        PlayerManager shootingPlayer = GameManager.GetPlayer(shooterId);
        // TODO: temp fix
        if (shootingPlayer == null)
        {
            BotManager shootingBot = GameManager.GetBot(shooterId);
            shootingBot.RpcGiveBotMoney(killVal, teamVal);
        }
        else
        {
            shootingPlayer.RpcGivePlayerMoney(killVal, teamVal);
        }
    }

    public void RpcGiveBotMoney(int killVal, int teamVal)
    {
        AdjustBotMoney(killVal);
        //do something with team money here
    }

    /// <summary>
    /// Make bot dead. Turn off certain components and reset some defaults.
    /// Currently this also causes them to respawn after a few seconds, but this
    /// will be controlled by rounds in the near future.
    /// </summary>
    public void KillBot()
    {
        isDead = true;
        this.GetComponent<Rigidbody>().useGravity = false;
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
    /// Respawn the bot after 3 seconds.
    /// This will be changed for when the round is over that is when the respawn will happen.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);
        //get position to spawn at
        Transform respawnPoint = this.GetComponent<BotSetup>().GetSpawnPoint();
        this.transform.position = respawnPoint.position;
        this.transform.rotation = respawnPoint.rotation;
        //wait for position to spawn at to be sent across network before spawning
        yield return new WaitForSeconds(0.1f);

        SetupBot();
    }

    /// <summary>
    /// Reset bot values to their default startng values.
    /// A part of respawning.
    /// </summary>
    public void SetDefaults()
    {
        //might not need this if statement
        this.GetComponent<Rigidbody>().useGravity = true;
        currentHealth = maxHealth;
        isDead = false;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        if (col != null)
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
    public int GetCurrentBotMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// Set the player's money to a whole new amount, a direct equal operation.
    /// </summary>
    /// <param name="newAmount">The new amount the player's money will be set to.</param>
    public void SetCurrentBotMoney(int newAmount)
    {
        currentMoney = newAmount;
    }

    /// <summary>
    /// Adjust the player's money by some amount. Note this is always the player's
    /// current money plus some given value. So to take away money, pass a negative
    /// number.
    /// </summary>
    /// <param name="val">Positive or negative integer representing how much to 
    /// change the player's current money by.</param>
    public void AdjustBotMoney(int val)
    {
        //TODO: put cap on money so it can't go above personal limit of this match setting
        currentMoney += val;
    }
}
