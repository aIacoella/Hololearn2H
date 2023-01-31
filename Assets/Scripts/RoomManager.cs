

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
        this.GenerateObjectsInWorld();
        photonView.RPC("OnGameStarted", RpcTarget.All);
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
        GameObject[] remainingObjects = GameObject.FindGameObjectsWithTag("ObjectsToBePlaced");
        Debug.Log("Remaining Object: " + remainingObjects.Length);

        List<GameObject> targets = remainingObjects.Where(obj =>
            obj.gameObject.GetComponent<ObjectManipulator>().enabled == true
        ).ToList();

        SortByDistance(targets);

        return targets[0];
    }

    public virtual GameObject GetClosestTarget()
    {
        GameObject draggedObject = VirtualAssistantManager.Instance.targetObject.gameObject;
        string tag = draggedObject.tag;

        List<GameObject> targets = GameObject.FindGameObjectsWithTag("Targets").ToList();

        SortByDistance(targets);
        return targets[0];
    }

    protected virtual Vector3 AdjustPositionWithSpatialMap(Vector3 position, Vector3 surfaceNormal)
    {
        Vector3 newPosition = position;

        /*
        RaycastHit hitInfo;
        float distance = 0.5f;

        // Check to see if there is a SpatialMapping mesh occluding the object at its current position.
        if (Physics.Raycast(position, surfaceNormal, out hitInfo, distance, SpatialMappingManager.Instance.LayerMask))
        {
            // If the object is occluded, reset its position.
            newPosition = hitInfo.point;
        }

        */
        return newPosition;
    }

}
