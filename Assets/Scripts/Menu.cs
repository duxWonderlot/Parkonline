using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen, CreateRoomScreen, LobbyScreen, lobbybrowserScreen;
    [Header("Main Screen")]
    public Button createRoomButton;
    public Button FindRoomButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerlistText;
    public TextMeshProUGUI roominfoText;
    public Button startGameButton;


    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButton = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();
    void Start()
    {
        createRoomButton.interactable = false;
        FindRoomButton.interactable = false;

        //enable the cursor sinces we hide it when we play the game
        Cursor.lockState = CursorLockMode.None;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;

        }
    }

    void SetScreen(GameObject screen)
    {
        //disable screens
        mainScreen.SetActive(false);
        CreateRoomScreen.SetActive(false);
        LobbyScreen.SetActive(false);
        lobbybrowserScreen.SetActive(false);
        //active the request screen
        screen.SetActive(true);

        if(screen == lobbybrowserScreen)
        {
            UpdateLobbyBrowerUI();
        }

    }
    
    public void OnBackButton()
    {
        SetScreen(mainScreen);
    }
    //Main Screen

    public void PlayerNameValueChange(TMP_InputField playernameInput)
    {
        PhotonNetwork.NickName = playernameInput.text;

    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        FindRoomButton.interactable = true;
    }

    public void OnCreatRoomButton()
    {
        SetScreen(CreateRoomScreen);
    }

    public void OnFindRoomButton()
    {
        SetScreen(lobbybrowserScreen);
    }

    //Create Room Screen

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        NetManger.instance.CreateRoom(roomNameInput.text);
    }

    //lobby screen
    public override void OnJoinedRoom()
    {
        SetScreen(LobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    [PunRPC]
    void UpdateLobbyUI()
    {
        //enable or disable start game button if host
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        playerlistText.text = "";
        foreach(Player player in PhotonNetwork.PlayerList)
      
            playerlistText.text += player.NickName + "\n";

        //set the room info text
        roominfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
       

    }

    public void OnstartGamebutton()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        NetManger.instance.photonView.RPC("ChangeScene", RpcTarget.All, "GameScene");
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    //Lobby Browser Screen
    GameObject createRoombutton()
    {
        GameObject buttonobj = Instantiate(roomButtonPrefab, roomListContainer.transform);
        roomButton.Add(buttonobj);

        return buttonobj;
    }


    void UpdateLobbyBrowerUI()
    {
        foreach(GameObject button in roomButton)
        {
            button.SetActive(false);
        }

        for(int x = 0; x<roomList.Count; ++x)
        {
            //get or create the button project
            GameObject button = x >= roomButton.Count ? createRoombutton() : roomButton[x];

            button.SetActive(true);

            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
            button.transform.Find("playercount").GetComponent<TextMeshProUGUI>().text = roomList[x].PlayerCount + " / " + roomList[x].MaxPlayers;


            //set the button onclick event

            Button buttoncomp = button.GetComponent<Button>();

            string roomName = roomList[x].Name;
            buttoncomp.onClick.RemoveAllListeners();

            buttoncomp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });
        }
    }

    public void OnJoinRoomButton(string roomName)
    {
        NetManger.instance.JoinRoom(roomName);
    }

    public void OnRefreshButton()
    {
        UpdateLobbyBrowerUI();
    }



    public override void OnRoomListUpdate(List<RoomInfo> allrooms)
    {
        roomList = allrooms;
    }
}

