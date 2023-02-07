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

public class MemoryManager : RoomManager
{
    public GameObject BoxPrefab;
    public GameObject[] ObjectsPrefabs;
    public GameObject PlayModesPrefabs;
    public GameObject[] MinionVirtualAssistantsPrefabs;
    public GameObject[] TYVirtualAssistantsPrefabs;
    public GameObject ElementsPrefab;
    public GameObject[] ObjectsToFindPrefabs;

    private int playMode;
    private int numberOfBoxes;
    private int waitingTime;
    private int assistantPresence;
    private int assistantBehaviour;
    private int selectedAssistant;
    private int assistantPatience;

    private Transform virtualAssistant;

    private Player[] photonPlayers;



    // Use this for initialization
    public void Start()
    {
        base.Start();

        this.cachePrefabs();

        LoadSettings();

        GameObject[] vaFamily = selectedAssistant == 0 ? MinionVirtualAssistantsPrefabs : TYVirtualAssistantsPrefabs;

        virtualAssistant = vaFamily[assistantBehaviour].transform;

        if (PhotonNetwork.IsMasterClient)
        {
            transform.GetChild(0).GetChild(playMode).gameObject.SetActive(true);
        }
    }

    [PunRPC]
    public override void OnGameStarted()
    {
        transform.GetChild(0).GetChild(playMode).gameObject.SetActive(true);

        PlayModeManager playModeManager = GetPlayModeManager();

        //Init Counter
        playModeManager.InitCounter();

        //Show Objects
        playModeManager.ShowObjects(waitingTime);
    }

    [PunRPC]
    public void AllignGameSettings(int playMode, int waitingTime)
    {
        this.playMode = playMode;
        this.waitingTime = waitingTime;
    }

    // Update is called once per frame
    public void Update()
    {

    }

    public override void GenerateObjectsInWorld()
    {
        //Seleziono il pavimento
        //Transform floor = SpatialProcessing.Instance.floors.ElementAt(0).transform;
        //SurfacePlane plane = floor.GetComponent<SurfacePlane>();


        //Vector3 floorPosition = floor.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
        //floorPosition = AdjustPositionWithSpatialMap(floorPosition, plane.SurfaceNormal);

        Transform anchorPosition = this.tableAnchor.transform;


        //Vector3 gazePosition = new Vector3(0f, 0f, 0f);
        //RaycastHit hitInfo;
        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, Physics.DefaultRaycastLayers))
        //{
        //    gazePosition = hitInfo.point;
        //}

        Vector3 boxesPosition = anchorPosition.position;
        boxesPosition.y = boxesPosition.y + 0.1f;

        //Vector3 relativePos = gazePosition - Camera.main.transform.position;
        //Quaternion rotation = Quaternion.LookRotation(relativePos);
        //rotation.x = 0f;
        //rotation.z = 0f;


        List<GameObject> objs = GetPlayModeManager().GenerateObjects(ObjectsPrefabs, numberOfBoxes);

        Debug.Log("Number of objects: " + objs.Count);

        //Transform sceneRoot = GameObject.Find("Broadcasted Content").transform;

        System.Random rnd = new System.Random();
        //Transform elems = new GameObject("Elements").transform;

        Transform elems = PhotonNetwork.Instantiate(ElementsPrefab.name, boxesPosition, Quaternion.identity).transform;

        List<int> list = new List<int>();
        for (int i = 1; i <= numberOfBoxes / 2; i++)
        {
            list.Add(i);
            list.Add(i);
        }

        //shuffle the list
        list = list.OrderBy(x => rnd.Next()).ToList();

        //elems.parent = sceneRoot;
        //elems.parent = GameObject.Find("SharedPlayground").transform.GetChild(0).transform;
        for (int i = 1; i <= numberOfBoxes / 2; i++)
        {
            int obj_index_1 = list.ElementAt(i * 2 - 2) - 1;
            int obj_index_2 = list.ElementAt(i * 2 - 1) - 1;

            GameObject obj = PhotonNetwork.Instantiate(objs.ElementAt(obj_index_1).name, new Vector3((float)Math.Pow(-1, i) * 0.3f * (i / 2), 0f, 0f) + boxesPosition, BoxPrefab.transform.rotation);
            GameObject obj2 = PhotonNetwork.Instantiate(objs.ElementAt(obj_index_2).name, new Vector3((float)Math.Pow(-1, i) * 0.3f * (i / 2), 0f, 0.3f) + boxesPosition, BoxPrefab.transform.rotation);
        }

        //elems.Translate(boxesPosition);
        //elems.Rotate(rotation.eulerAngles);


        //Vector3 assistantPosition = elems.GetChild(elems.childCount - 2).TransformPoint(0.3f * (float)Math.Pow(-1, elems.childCount / 2 % 2), 0f, 0f);
        Vector3 assistantPosition = elems.gameObject.transform.position + new Vector3(0f, 0f, -.9f);
        assistantPosition.y = anchorPosition.position.y;

        if (assistantPresence != 0)
        {
            PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
        }

        photonView.RPC("AllignGameSettings", RpcTarget.All, playMode, waitingTime);
    }


    private void LoadSettings()
    {
        playMode = MemorySettings.Instance.playMode;
        numberOfBoxes = MemorySettings.Instance.numberOfBoxes;
        waitingTime = MemorySettings.Instance.waitingTime;
        assistantPresence = VirtualAssistantChoice.Instance.assistantPresence;
        selectedAssistant = VirtualAssistantChoice.Instance.selectedAssistant;
        assistantPatience = VirtualAssistantSettings.Instance.assistantPatience;
    }


    public override void DestroyObjects()
    {
        if (VirtualAssistantManager.Instance != null)
        {
            Destroy(VirtualAssistantManager.Instance.gameObject);
        }
        Destroy(GameObject.Find("Elements"));
    }

    public int GetPlayMode()
    {
        return playMode;
    }

    public PlayModeManager GetPlayModeManager()
    {
        return transform.GetChild(0).GetChild(playMode).GetComponent<PlayModeManager>();
    }

    private void cachePrefabs()
    {
        // Allow prefabs not in a Resources folder
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            Debug.Log("Caching all prefabs");

            if (ObjectsPrefabs != null)
            {
                foreach (GameObject obj in ObjectsPrefabs)
                {
                    pool.ResourceCache.Add(obj.name, obj);
                }
            }

            if (ObjectsToFindPrefabs != null)
            {
                foreach (GameObject obj in ObjectsToFindPrefabs)
                {
                    pool.ResourceCache.Add(obj.name, obj);
                }
            }

            if (BoxPrefab != null)
            {
                pool.ResourceCache.Add(BoxPrefab.name, BoxPrefab);
            }

            if (ElementsPrefab != null)
            {
                pool.ResourceCache.Add(ElementsPrefab.name, ElementsPrefab);
            }

            if (MinionVirtualAssistantsPrefabs != null)
            {
                foreach (GameObject va in MinionVirtualAssistantsPrefabs)
                {
                    pool.ResourceCache.Add(va.name, va);
                }
            }

            if (TYVirtualAssistantsPrefabs != null)
            {
                foreach (GameObject va in TYVirtualAssistantsPrefabs)
                {
                    pool.ResourceCache.Add(va.name, va);
                }
            }

        }
    }

}
