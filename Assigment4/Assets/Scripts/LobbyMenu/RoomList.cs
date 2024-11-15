using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList instance;
    public GameObject roomManagerGameObject;
    public RoomManager roomManager;
    // Start is called before the first frame update
    [Header("UI")] public Transform roomListParent;
    public GameObject roomListPrefab;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomToCreateName(string roomName)
    {
        roomManager.roomNameToJoin = roomName;
    }
    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

            PhotonNetwork.ConnectUsingSettings();
        }
        
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (cachedRoomList.Count <= 0)
        {
            cachedRoomList = roomList;
        }
        else
        {
            foreach (var room in roomList)
            {
                for (int i = 0; i < cachedRoomList.Count; i++)
                {
                    if (room.Name == cachedRoomList[i].Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;
                        if (room.RemovedFromList)
                        {
                            newList.RemoveAt(i);
                        }
                        else
                        {
                            newList[i] = room;
                        }
                        cachedRoomList = newList;
                    }
                }
            }
            UpdateUI();
        }
        // Update is called once per frame
        void UpdateUI()
        {
            foreach (Transform child in roomListParent)
            {
                Destroy(child.gameObject);
            }
            foreach (var room in cachedRoomList)
            {
                GameObject roomItem =Instantiate(roomListPrefab, roomListParent);
                roomItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = room.Name;
                roomItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = room.PlayerCount + "/" + room.MaxPlayers;
                roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;

            }
        }
    }
    public void JoinRoom(string roomName)
    {
        roomManager.roomNameToJoin = roomName;
        roomManagerGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
