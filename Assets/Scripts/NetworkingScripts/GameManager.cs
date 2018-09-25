using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject worldCamera;

    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();
    private static Dictionary<string, BotManager> bots = new Dictionary<string, BotManager>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Two or more game managers found");
        }
        else
        {
            instance = this;
        }
    }

    public static void RegisterPlayer(string netId, PlayerManager player)
    {
        string playerID = Constants.PLAYER_ID_PREFIX + netId;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void RegisterBot(string netId, BotManager bot)
    {
        string botID = Constants.BOT_ID_PREFIX + netId;
        bots.Add(botID, bot);
        bot.transform.name = botID;
    }

    public static void DeRegisterPlayer(string netId)
    {
        players.Remove(Constants.PLAYER_ID_PREFIX + netId);
        //if(players.Count == 0)
        //{
        //    ReEnableWorldCam();
        //}
    }

    public static void DeRegisterBot(string netId)
    {
        bots.Remove(Constants.BOT_ID_PREFIX + netId);
    }

    public static void ReEnableWorldCam()
    {
        GameObject.FindGameObjectWithTag("WorldCamera").GetComponent<Camera>().enabled = true;
    }

    /// <summary>
    /// Returns a player based on their ID in the scene
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public static PlayerManager GetPlayer(string playerId)
    {
        return players[playerId];
    }

    /// <summary>
    /// Returns a bot manager based on their ID in the scene.
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    public static BotManager GetBot(string botId)
    {
        return bots[botId];
    }

    public void SetWorldCameraActive(bool isActive)
    {
        if (worldCamera == null)
        {
            return;
        }
        worldCamera.SetActive(isActive);
    }

}
