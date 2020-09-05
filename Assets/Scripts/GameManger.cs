using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
public class GameManger : MonoBehaviourPun
{
    [Header("Player")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnpoint;
    public int alivePlayer;

    private int playerInGame;

    public static GameManger instance;

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
    private void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayer = players.Length;

        photonView.RPC("IminGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void IminGame()
    {
        playerInGame++;

        if(PhotonNetwork.IsMasterClient && playerInGame == PhotonNetwork.PlayerList.Length)
        {
            photonView.RPC("SpawnPlayer", RpcTarget.All);
        }

    }
    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnpoint[Random.Range(0, spawnpoint.Length)].position, Quaternion.identity);

        //Initialize player all of them
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initalize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }

}
