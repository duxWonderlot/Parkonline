using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class NetManger : MonoBehaviourPunCallbacks
{

    public int maxplayer = 10;
    public static NetManger instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        //connect using master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to master server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room" +PhotonNetwork.CurrentRoom.Name);
    }


    public void CreateRoom (string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxplayer; // convert to byte
        PhotonNetwork.CreateRoom(roomName,options);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    [PunRPC]
    public void  ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }


}
