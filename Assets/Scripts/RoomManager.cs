

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.XR.ARFoundation;
using Photon.Realtime;
using Photon.Pun;

public abstract class RoomManager : Singleton<RoomManager>, IInRoomCallbacks
{
    public static RoomManager Room;

    [SerializeField] private GameObject photonUserPrefab = default;

    public GameObject tableAnchor;

    private Player[] photonPlayers;

    private int playersInRoom;
    private int myNumberInRoom;

    //Called by the host when all players connect to the room
    public abstract void GenerateObjectsInWorld();

    //Called by each client when he leaves the room
    public abstract void DestroyObjects();

    //Called by each client when game started RPC arrives (after GenerateObjectsInWorld)
    [PunRPC]
    public abstract void OnGameStarted();

    /* Room Logic */
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("On Player Entered Room");

        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;

        if (GameSettings.Instance.isMultiplayer && PhotonNetwork.IsMasterClient && playersInRoom == GameSettings.Instance.numPlayer)
        {
            this.startGame();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (Room == null)
        {
            Room = this;
        }
        else
        {
            if (Room != this)
            {
                Destroy(Room.gameObject);
                Room = this;
            }
        }

        //get the ar session origin component for the table anchor object in the scene
        ARSessionOrigin arSessionOrigin = GameObject.Find("TableAnchor").GetComponent<ARSessionOrigin>();

        //get the main camera
        Camera mainCamera = Camera.main;

        arSessionOrigin.camera = mainCamera;
    }

    public void Start()
    {
        /*
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);
        }
        */
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("On joined room");

        base.OnJoinedRoom();
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        CreatePlayer();
        if (!GameSettings.Instance.isMultiplayer)
        {
            this.startGame();
        }
    }

    private void CreatePlayer()
    {
        var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
    }


    //Only called by the host
    private void startGame()
    {
        //photonView.RPC("AllignWithAnchorPoint", RpcTarget.Others);
        //this.AllignWithAnchorPoint();
        this.GenerateObjectsInWorld();
        photonView.RPC("AllignWithAnchorPoint", RpcTarget.All);
        photonView.RPC("OnGameStarted", RpcTarget.All);

    }

    [PunRPC]
    public void AllignWithAnchorPoint()
    {
        Transform qrCodeTransform = QRCode.Instance.transform;

        Debug.Log("QR Code Position: " + qrCodeTransform.transform.position);
        Debug.Log("QR Code Rotation: " + qrCodeTransform.transform.rotation.eulerAngles);

        Quaternion rotationToApply = qrCodeTransform.transform.rotation * Quaternion.Euler(90, 0, 0);

        tableAnchor.transform.SetPositionAndRotation(qrCodeTransform.transform.position, rotationToApply);
        Debug.Log("Allignment completed: " + qrCodeTransform.transform.position + "\n" + rotationToApply.eulerAngles);
        Debug.Log("Child in table anchor: " + tableAnchor.transform.childCount);
    }

    /* Virtual Assistant Logic */
    protected void SortByDistance(List<GameObject> targets)
    {
        GameObject temp;
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = 0; j < targets.Count - 1; j++)
            {
                if (Vector3.Distance(targets.ElementAt(j).transform.position, VirtualAssistantManager.Instance.transform.position)
                    > Vector3.Distance(targets.ElementAt(j + 1).transform.position, VirtualAssistantManager.Instance.transform.position))
                {
                    temp = targets[j + 1];
                    targets[j + 1] = targets[j];
                    targets[j] = temp;
                }
            }
        }
    }

    public virtual GameObject GetClosestObject()
    {
        Rigidbody[] remainingObjects = GameObject.FindGameObjectWithTag("ObjectsToBePlaced").GetComponentsInChildren<Rigidbody>();
        List<GameObject> targets = new List<GameObject>();
        foreach (Rigidbody target in remainingObjects)
        {
            if (target.gameObject.GetComponent<ObjectManipulator>().enabled == true)
            {
                targets.Add(target.gameObject);
            }
        }

        SortByDistance(targets);

        return targets[0];
    }

    public virtual GameObject GetClosestTarget()
    {
        GameObject draggedObject = VirtualAssistantManager.Instance.targetObject.gameObject;
        Debug.Log(draggedObject);
        string tag = draggedObject.tag;

        Rigidbody[] placements = GameObject.FindGameObjectWithTag("Targets").GetComponentsInChildren<Rigidbody>();
        List<GameObject> targets = new List<GameObject>();
        foreach (Rigidbody target in placements)
        {
            if (target.gameObject.tag == tag)
            {
                targets.Add(target.gameObject);
            }
        }

        SortByDistance(targets);
        return targets[0];
    }

}
