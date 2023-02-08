

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
    public GameObject WeatherPrefabs;
    public GameObject ClothesPrefabs;
    public GameObject BagsPrefabs;
    public GameObject VirtualAssistantsPrefabs;
    public GameObject WeatherContainerPrefab;
    public GameObject ClothesContainerPrefab;
    public GameObject BagContainerPrefab;
    public GameObject TemperatureTextPrefab;


    private int numberOfLevel;
    private int numberOfClothes;
    private int assistantPresence;
    private int selectedAssistant;
    private int assistantBehaviour;
    private int assistantPatience;

    private Transform virtualAssistant;

    private string weathertag;
    private string temperaturetag;
    private int temperatureValue;
    // Use this for initialization
    public new void Start()
    {
        base.Start();

        cachePrefabs();

        LoadSettings();

        virtualAssistant = VirtualAssistantsPrefabs.transform.GetChild(selectedAssistant + 1).GetChild(assistantBehaviour - 1);
    }

    public override void GenerateObjectsInWorld()
    {
        //Seleziono il pavimento
        //Transform floor = SpatialProcessing.Instance.floors.ElementAt(0).transform;
        //SurfacePlane plane = floor.GetComponent<SurfacePlane>();

        Transform anchorPosition = this.tableAnchor.transform;
        System.Random rnd = new System.Random();

        //Vector3 floorPosition = floor.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
        //floorPosition = AdjustPositionWithSpatialMap(floorPosition, plane.SurfaceNormal);

        //Vector3 gazePosition = new Vector3(0f, 0f, 0f);
        //RaycastHit hitInfo;
        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, Physics.DefaultRaycastLayers))
        //{
        //    gazePosition = hitInfo.point;
        //}

        Vector3 weatherPosition = anchorPosition.position;
        weatherPosition.y = weatherPosition.y + 1f;
        Debug.DrawLine(Camera.main.transform.position, weatherPosition, Color.black, 30f);

        //Vector3 relativePos = Camera.main.transform.position - gazePosition;
        //Quaternion rotation = Quaternion.LookRotation(relativePos);
        //rotation.x = 0f;
        //rotation.z = 0f;

        //Transform sceneRoot = GameObject.Find("Broadcasted Content").transform;

        //Transform weather = new GameObject("Weather").transform;
        //weather.parent = sceneRoot;
        //weather.position = weatherPosition;
        Transform weather = PhotonNetwork.Instantiate(WeatherContainerPrefab.name, weatherPosition, Quaternion.identity).transform;

        Transform selectedLevel = WeatherPrefabs.transform.GetChild(numberOfLevel);
        Transform selectedWeather = selectedLevel.GetChild(rnd.Next(0, selectedLevel.childCount));
        //Instantiate(selectedWeather, weather.TransformPoint(-0.2f, 0f, 0f), rotation, weather);
        PhotonNetwork.Instantiate(selectedWeather.name, weather.TransformPoint(-0.2f, 0f, 0f), Quaternion.identity);

        //selectedWeather.GetChild(1).GetComponent<TemperatureGenerator>().GenerateTemperature();

        weathertag = GameObject.Find("Weather").transform.GetChild(0).GetChild(0).tag;
        temperaturetag = GameObject.Find("Weather").transform.GetChild(0).GetChild(1).tag;

        photonView.RPC("setRemoteTemperatureValue", RpcTarget.All, selectedWeather.GetChild(1).GetComponent<TemperatureGenerator>().GenerateTemperature());

        Debug.Log(weathertag + " - " + temperaturetag + " - " + temperatureValue);

        //Transform clothes = new GameObject("Clothes").transform;
        //clothes.parent = sceneRoot;
        //clothes.tag = "ObjectsToBePlaced";

        Vector3 clothesPosition = weatherPosition;
        clothesPosition.y = anchorPosition.position.y + 0.1f;
        clothesPosition.z = clothesPosition.z - 1f;
        Debug.DrawLine(weatherPosition, clothesPosition, Color.red, 30f);

        Transform clothes = PhotonNetwork.Instantiate(ClothesContainerPrefab.name, weatherPosition, Quaternion.identity).transform;

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
            //Instantiate(currentClothe.gameObject, currentClothe.position, currentClothe.rotation, clothes);
            PhotonNetwork.Instantiate(currentClothe.name, currentClothe.position, currentClothe.rotation);

            if (tags.Contains(weathertag) || tags.Contains(temperaturetag))
            {
                counter++;
            }
        }
        Debug.Log("Number of clothes: " + numberOfClothes + ", correct clothes: " + counter);

        //clothes.Translate(clothesPosition);
        //clothes.Rotate(rotation.eulerAngles);

        //Transform bag = new GameObject("Bag").transform;
        //bag.parent = sceneRoot;
        //bag.tag = "Targets";

        Vector3 bagPosition = weatherPosition;
        bagPosition.y = anchorPosition.position.y + 0.1f;

        Transform bag = PhotonNetwork.Instantiate(BagContainerPrefab.name, bagPosition, Quaternion.identity).transform;

        //Instantiate(BagsPrefabs.transform.GetChild(rnd.Next(0, BagsPrefabs.transform.childCount)).gameObject, bagPosition, rotation, bag);
        PhotonNetwork.Instantiate(BagsPrefabs.transform.GetChild(rnd.Next(0, BagsPrefabs.transform.childCount)).name, bagPosition, Quaternion.identity);

        Debug.DrawLine(clothesPosition, bagPosition, Color.blue, 30f);

        //Counter.Instance.InitializeCounter(counter);

        /*
                Vector3 assistantPosition = clothes.TransformPoint(-0.3f, 0f, -0.5f);

                assistantPosition.y = anchorPosition.position.y;

                Debug.DrawLine(bagPosition, assistantPosition, Color.green, 30f);

                if (assistantPresence != 0)
                {
                    //Instantiate(virtualAssistant.gameObject, assistantPosition, virtualAssistant.transform.rotation, sceneRoot);
                    PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

                    VirtualAssistantManager.Instance.patience = assistantPatience;
                    VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
                }
        */
    }

    [PunRPC]
    public override void OnGameStarted()
    {
        //Set weather and temperature tag
        GameObject weatherObj = GameObject.FindGameObjectWithTag("Weather");

        weathertag = weatherObj.transform.GetChild(0).tag;
        temperaturetag = weatherObj.transform.GetChild(1).tag;

        //Display Temperature
        weatherObj.transform.GetChild(1).GetComponent<TemperatureGenerator>().DisplayTemperature();

        Transform clothes = GameObject.Find("Clothes").transform;
        int counter = clothes.GetComponentsInChildren<TagsContainer>().Where(tc => tc.tags.Contains(weathertag) || tc.tags.Contains(temperaturetag)).Count();

        Counter.Instance.InitializeCounter(counter);

        initVirtualAssistant();
    }

    private void initVirtualAssistant()
    {
        Vector3 targetPosition = this.tableAnchor.transform.position;

        Vector3 assistantPosition = Vector3.Lerp(Camera.main.transform.position, targetPosition, 0.5f);

        assistantPosition.y = this.tableAnchor.transform.position.y;

        Debug.DrawLine(targetPosition, assistantPosition, Color.green, 30f);

        if (assistantPresence != 0)
        {
            Instantiate(virtualAssistant.gameObject, assistantPosition, virtualAssistant.transform.rotation, this.tableAnchor.transform);
            //PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
        }
    }

    [PunRPC]
    public void setRemoteTemperatureValue(int temperatureValue)
    {
        this.temperatureValue = temperatureValue;
    }

    public int getTemperatureValue()
    {
        return this.temperatureValue;
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

    public string getTemerature()
    {
        return temperaturetag;
    }

    public string getWeather()
    {
        return weathertag;
    }



    private void cachePrefabs()
    {
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            Debug.Log("Caching all prefabs");

            if (WeatherPrefabs != null)
            {
                foreach (Transform weatherLvl in WeatherPrefabs.transform)
                {
                    if (weatherLvl.transform.childCount > 0)
                    {
                        foreach (Transform weather in weatherLvl)
                        {
                            if (!pool.ResourceCache.ContainsKey(weather.name))
                            {
                                Debug.Log(weather.name);
                                pool.ResourceCache.Add(weather.name, weather.gameObject);
                            }
                        }
                    }
                }
            }

            if (ClothesPrefabs != null)
            {
                foreach (Transform clothe in ClothesPrefabs.transform)
                {
                    if (!pool.ResourceCache.ContainsKey(clothe.name))
                    {
                        pool.ResourceCache.Add(clothe.name, clothe.gameObject);
                    }
                }
            }

            if (BagsPrefabs != null)
            {
                foreach (Transform clothe in BagsPrefabs.transform)
                {
                    if (!pool.ResourceCache.ContainsKey(clothe.name))
                    {
                        pool.ResourceCache.Add(clothe.name, clothe.gameObject);
                    }
                }
            }

            if (VirtualAssistantsPrefabs != null)
            {
                Transform[] VirtualAssistanModels = { VirtualAssistantsPrefabs.transform.Find("Minion"), VirtualAssistantsPrefabs.transform.Find("Ty") };
                foreach (Transform virtualAssistantModel in VirtualAssistanModels)
                {
                    foreach (Transform virtualAssistant in virtualAssistantModel)
                    {
                        if (virtualAssistant.GetComponent<PhotonView>() != null && !pool.ResourceCache.ContainsKey(virtualAssistant.name))
                        {
                            pool.ResourceCache.Add(virtualAssistant.name, virtualAssistant.gameObject);
                        }
                    }
                }
            }

            if (!pool.ResourceCache.ContainsKey(WeatherContainerPrefab.name))
            {
                pool.ResourceCache.Add(WeatherContainerPrefab.name, WeatherContainerPrefab);
            }

            if (!pool.ResourceCache.ContainsKey(ClothesContainerPrefab.name))
            {
                pool.ResourceCache.Add(ClothesContainerPrefab.name, ClothesContainerPrefab);
            }

            if (!pool.ResourceCache.ContainsKey(BagContainerPrefab.name))
            {
                pool.ResourceCache.Add(BagContainerPrefab.name, BagContainerPrefab);
            }

        }

    }
}
