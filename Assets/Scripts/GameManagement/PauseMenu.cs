using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    private NetworkManager network;

    // Use this for initialization
    void Start()
    {
        network = NetworkManager.singleton;
    }

    
    public void LeaveRoom()
    {
        MatchInfo matchInfo = network.matchInfo;
        network.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, network.OnDropConnection);
        network.StopHost();
    }
}
