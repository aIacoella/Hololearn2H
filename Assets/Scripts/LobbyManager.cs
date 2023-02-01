using UnityEngine;
using Photon.Pun;
using MRTK.Tutorials.MultiUserCapabilities;
using Photon.Realtime;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.SampleQRCodes;
using System.Collections.Generic;

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

    struct ActionData
    {
        public enum Type
        {
            Added,
            Updated,
            Removed
        };
        public Type type;
        public Microsoft.MixedReality.QR.QRCode qrCode;

        public ActionData(Type type, Microsoft.MixedReality.QR.QRCode qRCode) : this()
        {
            this.type = type;
            qrCode = qRCode;
        }
    }

    private Queue<ActionData> pendingActions = new Queue<ActionData>();

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

        QRCodesManager.Instance.QRCodeAdded += Instance_QRCodeAdded;
        QRCodesManager.Instance.QRCodeUpdated += Instance_QRCodeUpdated;

        tableAnchor = GameObject.Find("TableAnchor");

        debug.GetComponent<TextMesh>().text = "Waiting for QR Code";
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        this.isConnectedToMaster = true;
    }

    private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        Debug.Log("QRCodesVisualizer Instance_QRCodeAdded");

        lock (pendingActions)
        {
            pendingActions.Enqueue(new ActionData(ActionData.Type.Added, e.Data));
        }
    }

    private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        Debug.Log("QRCodesVisualizer Instance_QRCodeUpdated");

        lock (pendingActions)
        {
            pendingActions.Enqueue(new ActionData(ActionData.Type.Updated, e.Data));
        }
    }

    private void OnQRCodeUpdated(QRCode qrCode)
    {
        string CodeText = qrCode.Data;

        if (CodeText == QR_CODE_TEXT)
        {
            Debug.Log("UpdatingHoloLearn2 QR Code " + qrCode.Id);

            try
            {
                tableAnchor.GetComponent<SpatialGraphNodeTracker>().Id = qrCode.Id;
            }
            catch (System.Exception error)
            {
                Debug.LogWarning(error.Message);
            }

            Debug.Log(tableAnchor);
        }
    }


    private void OnQRCodeAdded(QRCode qrCode)
    {
        string CodeText = qrCode.Data;

        if (CodeText == QR_CODE_TEXT)
        {
            Debug.Log("HoloLearn2 QR Code Found! " + qrCode.Id);
            try
            {
                tableAnchor.GetComponent<SpatialGraphNodeTracker>().Id = qrCode.Id;
            }
            catch (System.Exception error)
            {
                Debug.LogWarning(error.Message);
            }

            Debug.Log("What is table anchor? " + tableAnchor);
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

    private void HandleEvents()
    {
        lock (pendingActions)
        {
            while (pendingActions.Count > 0)
            {
                var action = pendingActions.Dequeue();
                if (action.type == ActionData.Type.Added)
                {
                    OnQRCodeAdded(action.qrCode);
                }
                else if (action.type == ActionData.Type.Updated)
                {
                    OnQRCodeUpdated(action.qrCode);
                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleEvents();
    }
}
