using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using MRTK.Tutorials.MultiUserCapabilities;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public static LobbyManager lobby;

    private int roomNumber = 1;

    private int userIdCount;

    private void Awake()
    {
        if (lobby == null)
        {
            lobby = this;
        }
        else
        {
            if (lobby != this)
            {
                Destroy(lobby.gameObject);
                lobby = this;
            }
        }

        DontDestroyOnLoad(gameObject);

        GenericNetworkManager.OnReadyToStartNetwork += StartNetwork;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");

        var randomUserId = Random.Range(0, 999999);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = randomUserId.ToString();
        userIdCount++;
        PhotonNetwork.NickName = PhotonNetwork.AuthValues.UserId;
        //PhotonNetwork.JoinRoom(GameSettings.Instance.getSceneName());
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("\nPhotonLobby.OnJoinedRoom()");
        Debug.Log("Current room name: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Other players in room: " + PhotonNetwork.CountOfPlayersInRooms);
        Debug.Log("Total players in room: " + (PhotonNetwork.CountOfPlayersInRooms + 1));
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating Room");

        var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = (byte)(GameSettings.Instance.numPlayer) };
        PhotonNetwork.CreateRoom(GameSettings.Instance.getSceneName(), roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");

        base.OnCreatedRoom();
        roomNumber++;
    }

    public void OnCancelButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void StartNetwork()
    {
        PhotonNetwork.ConnectUsingSettings();
        lobby = this;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
