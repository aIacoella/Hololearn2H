using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using MRTK.Tutorials.MultiUserCapabilities;
using Photon.Realtime;
using Microsoft.MixedReality.QR;
using Microsoft.MixedReality.SampleQRCodes;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.OpenXR;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    public static LobbyManager lobby;

    public GameObject tableAnchor;

    public GameObject debug;

    private int roomNumber = 1;

    private int userIdCount;

    //0 = scene loaded, 1 = QR Code Scanned => Joined/Created
    private bool isConnectedToMaster = false;

    private SpatialGraphNode node;

    private System.Guid Id { get; set; }

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
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master");
        this.isConnectedToMaster = true;
    }

    private void OnQRCodeAdded(object sender, QRCodeEventArgs<Microsoft.MixedReality.QR.QRCode> e)
    {
        QRCode qrCode = e.Data;
        string CodeText = qrCode.Data;
        Debug.Log("New QR Code Added: " + CodeText);

        debug.GetComponent<TextMesh>().text = CodeText;

        if (node == null || node.Id != Id)
        {
            node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
            Debug.Log("Initialize SpatialGraphNode Id= " + Id);
        }

        if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
        {
            if (CameraCache.Main.transform.parent != null)
            {
                pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
            }

            Debug.Log("Id= " + Id + " QRPose = " + pose.position.ToString("F7") + " QRRot = " + pose.rotation.ToString("F7"));
        }
        else
        {
            Debug.LogWarning("Cannot locate " + Id);
        }

    }

    public void OnQRCodeScanned()
    {
        //To be replaced with QR Code
        tableAnchor.transform.position = Camera.main.transform.position;

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
