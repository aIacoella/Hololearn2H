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

public class DressUpManager : RoomManager
{
    public GameObject[] WheatherPrefabsLvl1;

    public GameObject[] WheatherPrefabsLvl2;

    public GameObject TemperatureTextPrefab;
    public GameObject[] ClothesPrefabs;
    public GameObject[] BagsPrefabs;
    public GameObject[] VirtualAssistantsPrefabs;

    private int numberOfLevel;
    private int numberOfClothes;
    private int assistantPresence;
    private int selectedAssistant;
    private int assistantBehaviour;
    private int assistantPatience;

    private Transform virtualAssistant;

    private string weathertag;
    private string temperaturetag;



    // Use this for initialization
    public void Start()
    {
        base.Start();

        // Allow prefabs not in a Resources folder
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            Debug.Log("Caching all prefabs");

            if (WheatherPrefabsLvl1 != null)
            {
                foreach (GameObject wheather in WheatherPrefabsLvl1)
                {
                    pool.ResourceCache.Add(wheather.name, wheather);
                }
            }

            if (WheatherPrefabsLvl2 != null)
            {
                foreach (GameObject wheather in WheatherPrefabsLvl2)
                {
                    if (!pool.ResourceCache.ContainsKey(wheather.name))
                        pool.ResourceCache.Add(wheather.name, wheather);
                }
            }

            if (TemperatureTextPrefab != null) pool.ResourceCache.Add(TemperatureTextPrefab.name, TemperatureTextPrefab);

            if (ClothesPrefabs != null)
            {
                foreach (GameObject clothes in ClothesPrefabs)
                {
                    pool.ResourceCache.Add(clothes.name, clothes);
                }
            }

            if (BagsPrefabs != null)
            {
                foreach (GameObject bags in BagsPrefabs)
                {
                    pool.ResourceCache.Add(bags.name, bags);
                }
            }

            if (VirtualAssistantsPrefabs != null)
            {
                foreach (GameObject va in VirtualAssistantsPrefabs)
                {
                    pool.ResourceCache.Add(va.name, va);
                }
            }
        }



        LoadSettings();

        virtualAssistant = VirtualAssistantsPrefabs[selectedAssistant].transform.GetChild(assistantBehaviour - 1);
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

        Transform weather = new GameObject("Weather").transform;
        weather.parent = sceneRoot;
        weather.position = weatherPosition;

        GameObject[] selectedLevel = numberOfLevel == 1 ? WheatherPrefabsLvl1 : WheatherPrefabsLvl2;
        GameObject selectedWeatherObj = selectedLevel[rnd.Next(0, selectedLevel.Length)];
        //Instantiate(selectedWeather, weather.TransformPoint(-0.2f, 0f, 0f), rotation, weather);
        GameObject selectedWeather = PhotonNetwork.Instantiate(selectedWeatherObj.transform.name, weather.TransformPoint(-0.2f, 0f, 0f), rotation, 0);


        TemperatureGenerator temperatureGenerator = selectedWeather.transform.GetChild(1).GetComponent<TemperatureGenerator>();
        selectedWeather.transform.GetChild(1).GetComponent<TemperatureGenerator>().GenerateTemperature();

        weathertag = selectedWeather.transform.GetChild(0).tag;
        temperaturetag = selectedWeather.transform.GetChild(1).tag;

        Debug.Log("Weather: " + weathertag);
        Debug.Log("Temperature: " + temperaturetag);

        Vector3 clothesPosition = weatherPosition;
        clothesPosition.y = floorPosition.y + 0.1f;
        Debug.DrawLine(weatherPosition, clothesPosition, Color.red, 30f);

        int counter = 0;
        for (int i = 0; i < numberOfClothes; i++)
        {
            Transform currentClothe = ClothesPrefabs[(rnd.Next(0, ClothesPrefabs.Length))].transform;
            List<string> tags = currentClothe.GetComponent<TagsContainer>().tags;
            currentClothe.tag = "ObjectsToBePlaced";

            if (counter <= Math.Floor((double)numberOfClothes / 3))
            {
                if (!tags.Contains(weathertag) && !tags.Contains(temperaturetag))
                {
                    i--;
                    continue;
                }
            }
            PhotonNetwork.Instantiate(currentClothe.name, currentClothe.position, currentClothe.rotation);

            if (tags.Contains(weathertag) || tags.Contains(temperaturetag))
            {
                counter++;
            }
        }
        Debug.Log("Number of clothes: " + numberOfClothes + ", correct clothes: " + counter);

        Transform bag = new GameObject("Bag").transform;
        bag.parent = sceneRoot;
        bag.tag = "Targets";

        Vector3 bagPosition = weatherPosition;
        bagPosition.y = floorPosition.y + 0.1f;

        PhotonNetwork.Instantiate(BagsPrefabs[(rnd.Next(0, BagsPrefabs.Length))].transform.name, bagPosition, rotation);

        Debug.DrawLine(clothesPosition, bagPosition, Color.blue, 30f);

        Counter.Instance.InitializeCounter(counter);

        Vector3 assistantPosition = new Vector3(-0.3f, 0f, 0.3f) + bagPosition;
        assistantPosition.y = floor.position.y;
        Debug.DrawLine(bagPosition, assistantPosition, Color.green, 30f);

        if (assistantPresence != 0)
        {
            PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
        }

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
