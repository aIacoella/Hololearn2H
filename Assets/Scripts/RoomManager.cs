

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

    private Transform taskAnchorPosition = default;

    private Player[] photonPlayers;

    private int playersInRoom;
    private int myNumberInRoom;

    public abstract void GenerateObjectsInWorld();

    public abstract void DestroyObjects();


    /* Room Logic */
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("On Player Entered Room");

        base.OnPlayerEnteredRoom(newPlayer);
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom++;
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
            this.GenerateObjectsInWorld();
        }
    }

    private void CreatePlayer()
    {
        var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
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
        //Debug.Log(draggedObject);
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
