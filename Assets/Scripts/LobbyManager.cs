using UnityEngine;
using Photon.Pun;
using MRTK.Tutorials.MultiUserCapabilities;
using Photon.Realtime;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.SampleQRCodes;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public static LobbyManager lobby;

    public GameObject tableAnchor;

    public GameObject debug;

    private int roomNumber = 1;

    private int userIdCount;

    //0 = scene loaded, 1 = QR Code Scanned => Joined/Created
    private bool isConnectedToMaster = false;

    private const string QR_CODE_TEXT = "HoloLearn2";

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
        QRCodesManager.Instance.QRCodeAdded += OnQRCodeAdded;
        QRCodesManager.Instance.QRCodeUpdated += OnQRCodeUpdated;

        debug.GetComponent<TextMesh>().text = "Waiting for QR Code";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        this.isConnectedToMaster = true;
    }

    private void OnQRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        QRCode qrCode = e.Data;
        string CodeText = qrCode.Data;

        if (CodeText == QR_CODE_TEXT)
        {
            Debug.Log("UpdatingHoloLearn2 QR Code " + qrCode.Id);

            try
            {
                GameObject.Find("TableAnchor").GetComponent<SpatialGraphNodeTracker>().Id = qrCode.Id;
            }
            catch (System.Exception error)
            {
                Debug.LogWarning(error.Message);
            }

            Debug.Log(tableAnchor);
        }
    }


    private void OnQRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        QRCode qrCode = e.Data;
        string CodeText = qrCode.Data;

        if (CodeText == QR_CODE_TEXT)
        {
            Debug.Log("HoloLearn2 QR Code Found! " + qrCode.Id);
            tableAnchor.GetComponent<SpatialGraphNodeTracker>().Id = qrCode.Id;
        }
        else
        {
            Debug.Log("QR Code Found but text: " + CodeText);

        }
    }

    public void OnQRCodeScanned()
    {
        //To be replaced with QR Code
        //tableAnchor.transform.position = Camera.main.transform.position;

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
