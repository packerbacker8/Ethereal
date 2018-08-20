using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public MatchSettings matchSettings;

    private const string PLAYER_ID_PREFIX = "User_";

    private static Dictionary<string, PlayerManager> players = new Dictionary<string, PlayerManager>();

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Two or game managers find");
        }
        else
        {
            instance = this;
        }
    }

    public static void RegisterPlayer(string netId, PlayerManager player)
    {
        string playerID = PLAYER_ID_PREFIX + netId;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static void DeRegisterPlayer(string netId)
    {
        players.Remove(PLAYER_ID_PREFIX + netId);
        if(players.Count == 0)
        {
            ReEnableWorldCam();
        }
    }

    public static void ReEnableWorldCam()
    {
        GameObject.FindGameObjectWithTag("WorldCamera").GetComponent<Camera>().enabled = true;
    }


    public static PlayerManager GetPlayer(string playerId)
    {
        return players[playerId];
    }

}
