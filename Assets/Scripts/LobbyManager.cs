using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MRTK.Tutorials.MultiUserCapabilities;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static LobbyManager lobby;

    public GameObject ScanQRCodeOverlay;

    public GameObject GameOverOverlay;

    private int roomNumber = 1;

    private int userIdCount;

    //0 = scene loaded, 1 = QR Code Scanned => Joined/Created
    private bool isConnectedToMaster = false;

    private GameObject tableAnchor;

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

    public void Start()
    {
        tableAnchor = GameObject.Find("TableAnchor");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        this.isConnectedToMaster = true;

        if (QRCode.Instance.isInitialized)
        {
            ScanQRCodeOverlay.SetActive(false);
            JoinOrCreateRoomIntent();
        }
    }

    public void GameOver()
    {
        GameOverOverlay.SetActive(true);
    }

    public void QuitGame()
    {
        //Leave Room
        try
        {
            PhotonNetwork.LeaveRoom();
        }
        catch (System.Exception)
        {
            Debug.Log("Failed to leave room");
        }

        //Disconnect from master
        try
        {
            PhotonNetwork.Disconnect();
        }
        catch (System.Exception)
        {
            Debug.Log("Failed to disconnect from master");
        }
        //Delete SharedPlayground
        Destroy(GameObject.Find("SharedPlayground"));

        //Open MainMenu
        BrodcastedContent.Instance.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

        //Delete this
        Destroy(gameObject);
    }


    public void JoinOrCreateRoomIntent()
    {
        Debug.Log("Join room intent");

        if (!isConnectedToMaster)
        {
            throw new System.Exception("Connection to master failed before QR Code scan");
        }

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
        Debug.LogWarning(message);
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning(message);
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating Room");

        var roomOptions = new RoomOptions { IsVisible = true, IsOpen = true, MaxPlayers = (byte)(GameSettings.Instance.numPlayer) };
        PhotonNetwork.CreateRoom(GameSettings.Instance.getSceneName() + Random.Range(0, 999999), roomOptions);
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

    // Update is called once per frame
    void Update()
    {
    }
}
