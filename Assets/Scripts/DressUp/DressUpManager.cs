﻿

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

public class DressUpManager : TaskManager, IInRoomCallbacks
{
    public GameObject WeatherPrefabs;
    public GameObject[] ClothesPrefabs;
    public GameObject BagsPrefabs;
    public GameObject VirtualAssistantsPrefabs;

    private int numberOfLevel;
    private int numberOfClothes;
    private int assistantPresence;
    private int selectedAssistant;
    private int assistantBehaviour;
    private int assistantPatience;

    private Transform virtualAssistant;

    private string weathertag;
    private string temperaturetag;

    public static DressUpManager Room;

    [SerializeField] private GameObject photonUserPrefab = default;

    private Transform roverExplorerLocation = default;

    private Player[] photonPlayers;

    private int playersInRoom;
    private int myNumberInRoom;


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
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


    // Use this for initialization
    public override void Start()
    {
        // Allow prefabs not in a Resources folder
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            if (photonUserPrefab != null) pool.ResourceCache.Add(photonUserPrefab.name, photonUserPrefab);

            if (WeatherPrefabs != null) pool.ResourceCache.Add(WeatherPrefabs.name, WeatherPrefabs);

            //if (ClothesPrefabs != null) pool.ResourceCache.Add(ClothesPrefabs.name, ClothesPrefabs);
            if (ClothesPrefabs != null)
            {
                foreach (GameObject clothes in ClothesPrefabs)
                {
                    pool.ResourceCache.Add(clothes.name, clothes);
                }
            }

            if (BagsPrefabs != null) pool.ResourceCache.Add(BagsPrefabs.name, BagsPrefabs);

            if (VirtualAssistantsPrefabs != null) pool.ResourceCache.Add(VirtualAssistantsPrefabs.name, VirtualAssistantsPrefabs);
        }


        LoadSettings();

        virtualAssistant = VirtualAssistantsPrefabs.transform.GetChild(selectedAssistant + 1).GetChild(assistantBehaviour - 1);



    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        photonPlayers = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayers.Length;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        StartGame();
    }

    private void StartGame()
    {
        CreatePlayer();

        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.Instantiate(ClothesPrefabs[0].name, Vector3.zero, Quaternion.identity);

        Debug.Log("Test master");

        //GameObject.Find("TaskMenu").GetComponent<TaskInteractionHandler>().OverrideAndStartPlaying();
    }

    private void CreatePlayer() {
        var player = PhotonNetwork.Instantiate(photonUserPrefab.name, Vector3.zero, Quaternion.identity);
    }



    public override void GenerateObjectsInWorld()
    {
        //Seleziono il pavimento
        //Transform floor = SpatialProcessing.Instance.floors.ElementAt(0).transform;
        //SurfacePlane plane = floor.GetComponent<SurfacePlane>();
        

        System.Random rnd = new System.Random();


        //Vector3 floorPosition = floor.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
        //floorPosition = AdjustPositionWithSpatialMap(floorPosition, plane.SurfaceNormal);

        //TODO: Remove This
        Transform floor = gameObject.transform;
        floor.position = Vector3.zero;
        Vector3 floorPosition = Vector3.zero;
        //TODO: Remove this

        Vector3 gazePosition = new Vector3(0f, 0f, 0f);
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, Physics.DefaultRaycastLayers))
        {
            gazePosition = hitInfo.point;
        }

        Vector3 weatherPosition = gazePosition;
        weatherPosition.y = floorPosition.y + 1f;
        Debug.DrawLine(Camera.main.transform.position, weatherPosition, Color.black, 30f);


        //Vector3 relativePos = Camera.main.transform.position - gazePosition;
        Vector3 relativePos = Camera.main.transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = 0f;
        rotation.z = 0f;


        Transform sceneRoot = GameObject.Find("Broadcasted Content").transform;

        //PhotonNetwork.Instantiate(ClothesPrefabs.name, weatherPosition, rotation);

       /* Transform weather = new GameObject("Weather").transform;
        weather.parent = sceneRoot;
        weather.position = weatherPosition;

        Transform selectedLevel = WeatherPrefabs.transform.GetChild(numberOfLevel);
        Transform selectedWeather = selectedLevel.GetChild(rnd.Next(0, selectedLevel.childCount));
        Instantiate(selectedWeather, weather.TransformPoint(-0.2f, 0f, 0f), rotation, weather);

        selectedWeather.GetChild(1).GetComponent<TemperatureGenerator>().GenerateTemperature();


        weathertag = GameObject.Find("Weather").transform.GetChild(0).GetChild(0).tag;
        temperaturetag = GameObject.Find("Weather").transform.GetChild(0).GetChild(1).tag;

        Transform clothes = new GameObject("Clothes").transform;
        clothes.parent = sceneRoot;
        clothes.tag = "ObjectsToBePlaced";

        Vector3 clothesPosition = weatherPosition;
        clothesPosition.y = floorPosition.y + 0.1f;
        Debug.DrawLine(weatherPosition, clothesPosition, Color.red, 30f);

        int counter = 0;
        for (int i = 0; i < numberOfClothes; i++)
        {
            Transform currentClothe = ClothesPrefabs.transform.GetChild(rnd.Next(0, ClothesPrefabs.transform.childCount));
            List<string> tags = currentClothe.GetComponent<TagsContainer>().tags;

            if (counter <= Math.Floor((double)numberOfClothes / 3))
            {
                if (!tags.Contains(weathertag) && !tags.Contains(temperaturetag))
                {
                    i--;
                    continue;
                }
            }
            Instantiate(currentClothe.gameObject, currentClothe.position, currentClothe.rotation, clothes);

            if (tags.Contains(weathertag) || tags.Contains(temperaturetag))
            {
                counter++;
            }
        }
        Debug.Log("Number of clothes: " + numberOfClothes + ", correct clothes: " + counter);

        clothes.Translate(clothesPosition);
        clothes.Rotate(rotation.eulerAngles);


        Transform bag = new GameObject("Bag").transform;
        bag.parent = sceneRoot;
        bag.tag = "Targets";

        Vector3 bagPosition = weatherPosition;
        bagPosition.y = floorPosition.y + 0.1f;
        Instantiate(BagsPrefabs.transform.GetChild(rnd.Next(0, BagsPrefabs.transform.childCount)).gameObject, bagPosition, rotation, bag);
        Debug.DrawLine(clothesPosition, bagPosition, Color.blue, 30f);


        Counter.Instance.InitializeCounter(counter);


        Vector3 assistantPosition = clothes.TransformPoint(-0.3f, 0f, 0.3f);
        assistantPosition.y = floor.position.y;
        Debug.DrawLine(bagPosition, assistantPosition, Color.green, 30f);

        if (assistantPresence != 0)
        {
            Instantiate(virtualAssistant.gameObject, assistantPosition, virtualAssistant.transform.rotation, sceneRoot);
            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
        }*/

        
    }


    public override GameObject GetClosestObject()
    {
        Rigidbody[] remainingObjects = GameObject.FindGameObjectWithTag("ObjectsToBePlaced").GetComponentsInChildren<Rigidbody>();
        List<GameObject> targets = new List<GameObject>();

        foreach (Rigidbody target in remainingObjects)
        {
            List<string> tags = target.gameObject.GetComponent<TagsContainer>().tags;
            if (target.gameObject.GetComponent<ObjectManipulator>().enabled == true &&
                (tags.Contains(weathertag) || tags.Contains(weathertag)))
            {
                targets.Add(target.gameObject);
            }
        }

        SortByDistance(targets);

        return targets[0];
    }


    private void LoadSettings()
    {
        numberOfLevel = DressUpSettings.Instance.numberOfLevel;
        numberOfClothes = DressUpSettings.Instance.numberOfClothes;
        assistantPresence = VirtualAssistantChoice.Instance.assistantPresence;
        selectedAssistant = VirtualAssistantChoice.Instance.selectedAssistant;
        assistantBehaviour = VirtualAssistantSettings.Instance.assistantBehaviour;
        assistantPatience = VirtualAssistantSettings.Instance.assistantPatience;
    }


    public override void DestroyObjects()
    {
        if (VirtualAssistantManager.Instance != null)
        {
            Destroy(VirtualAssistantManager.Instance.gameObject);
        }
        Destroy(GameObject.Find("Weather"));
        Destroy(GameObject.Find("Clothes"));
        Destroy(GameObject.Find("Bag"));
    }
}
