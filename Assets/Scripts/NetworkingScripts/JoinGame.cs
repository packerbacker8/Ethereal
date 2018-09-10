using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour
{
    private List<GameObject> rooms = new List<GameObject>();

    private NetworkManager network;

    [SerializeField]
    private Text statusText;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListContent;

    private void Start()
    {
        network = NetworkManager.singleton;
        if(network.matchMaker == null)
        {
            network.StartMatchMaker();
        }

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        network.matchMaker.ListMatches(0, 20, "", false, 0, 0, OnMatchList);
        statusText.text = "Loading";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        statusText.text = "";

        if (!success)
        {
            statusText.text = "Failed to retrieve rooms available";
            Debug.LogError(extendedInfo);
        }
        else
        {
            foreach(MatchInfoSnapshot match in matches)
            {
                GameObject roomListGameObj = Instantiate(roomListItemPrefab, roomListContent);
                RoomListItem roomListItem = roomListGameObj.GetComponent<RoomListItem>();
                if(roomListItem != null)
                {
                    roomListItem.Setup(match, JoinRoom);
                }
                rooms.Add(roomListGameObj);
            }
            if(rooms.Count == 0)
            {
                statusText.text = "No rooms available";
            }
        }
    }

    private void ClearRoomList()
    {
        foreach(GameObject room in rooms)
        {
            Destroy(room);
        }
        rooms.Clear();
    }

    public void JoinRoom(MatchInfoSnapshot matchToJoin)
    {
        Debug.Log("Joining " + matchToJoin.name);
        statusText.text = "Joining";
        network.matchMaker.JoinMatch(matchToJoin.networkId, "", "", "", 0, 0, network.OnMatchJoined);
    }
}
