using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    public GameObject player;
    public string roomNameToJoin = "test";
    public void JoinRoomOnButtonPressed() {
        Debug.Log("Connecting...");
        PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, new Photon.Realtime.RoomOptions { MaxPlayers = 2 }, null);

    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We're in the room");
        PhotonNetwork.Instantiate(player.name, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
