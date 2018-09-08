using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : MonoBehaviour
{
    [SerializeField]
    private uint roomSize = 10;

    private string roomName;

    private NetworkManager network;

    private void Start()
    {
        network = NetworkManager.singleton;
        if(network.matchMaker == null)
        {
            network.StartMatchMaker();
        }
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    public void CreateRoom()
    {
        if (roomName  != "" && roomName != null)
        {
            Debug.Log("Creating room " + roomName + " with " + roomSize + " player spots available.");
            network.matchMaker.CreateMatch(roomName, roomSize, true, "", "", "", 0, 0,  network.OnMatchCreate);
        }
        else
        {
            Debug.LogError("Room name not a valid name.");
            Debug.LogError(roomName);
        }
    }
}
