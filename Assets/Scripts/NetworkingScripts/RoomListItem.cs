using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour
{
    public delegate void JoinRoomCallback(MatchInfoSnapshot theMatch);
    private JoinRoomCallback joinRoomCallback;

    private MatchInfoSnapshot match;

    [SerializeField]
    private Text roomNameText;

    public void Setup(MatchInfoSnapshot incMatch, JoinRoomCallback callback)
    {
        match = incMatch;
        joinRoomCallback = callback;
        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        joinRoomCallback.Invoke(match);
    }
}
