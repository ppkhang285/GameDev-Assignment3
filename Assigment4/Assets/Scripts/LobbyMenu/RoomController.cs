using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class RoomController : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;
    string lobbyScene = "LobbyMenu";
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Not in the room, returning back to Lobby");
            UnityEngine.SceneManagement.SceneManager.LoadScene(lobbyScene);
            return;
        }
        //We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        StartCoroutine(DelayedPlayerInstantiation());
    }
    private IEnumerator DelayedPlayerInstantiation()
    {
        yield return new WaitForSeconds(2.0f);
        // PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position, spawnPoints[Random.Range(0, spawnPoints.Length - 1)].rotation, 0);
    }
    void OnGUI()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }
        if (GUI.Button(new Rect(5, 5, 125, 25), "Leave room"))
        {
            PhotonNetwork.LeaveRoom();
        }
        GUI.Label(new Rect(135, 5, 200, 25), PhotonNetwork.CurrentRoom.Name);
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            string isMasterClient = (PhotonNetwork.PlayerList[i].IsMasterClient ? ": MasterClient" : "");
            GUI.Label(new Rect(5, 35 + 30 * i, 200, 25), PhotonNetwork.PlayerList[i].NickName + isMasterClient);
        }
    }
    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(lobbyScene);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if(PhotonNetwork.PlayerList.Length==2){
            PhotonNetwork.LoadLevel("NetworkGameplay");
        }
    }
}