

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
    public GameObject[] MinionVirtualAssistantsPrefabs;
    public GameObject[] TYVirtualAssistantsPrefabs;

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
    public new void Start()
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
                    if (!pool.ResourceCache.ContainsKey(wheather.name))
                    {
                        pool.ResourceCache.Add(wheather.name, wheather);
                    }
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

            if (TemperatureTextPrefab != null)
            {
                if (!pool.ResourceCache.ContainsKey(TemperatureTextPrefab.name))
                {
                    pool.ResourceCache.Add(TemperatureTextPrefab.name, TemperatureTextPrefab);
                }
            }

            if (ClothesPrefabs != null)
            {
                foreach (GameObject clothes in ClothesPrefabs)
                {
                    if (!pool.ResourceCache.ContainsKey(clothes.name))
                    {
                        pool.ResourceCache.Add(clothes.name, clothes);
                    }
                }
            }

            if (BagsPrefabs != null)
            {
                foreach (GameObject bags in BagsPrefabs)
                {
                    if (!pool.ResourceCache.ContainsKey(bags.name))
                    {
                        pool.ResourceCache.Add(bags.name, bags);
                    }
                }
            }

            if (MinionVirtualAssistantsPrefabs != null)
            {
                foreach (GameObject va in MinionVirtualAssistantsPrefabs)
                {
                    if (!pool.ResourceCache.ContainsKey(va.name))
                    {
                        pool.ResourceCache.Add(va.name, va);
                    }
                }
            }

            if (TYVirtualAssistantsPrefabs != null)
            {
                foreach (GameObject va in TYVirtualAssistantsPrefabs)
                {
                    if (!pool.ResourceCache.ContainsKey(va.name))
                    {
                        pool.ResourceCache.Add(va.name, va);
                    }
                }
            }
        }

        LoadSettings();

        GameObject[] vaFamily = selectedAssistant == 0 ? MinionVirtualAssistantsPrefabs : TYVirtualAssistantsPrefabs;
        virtualAssistant = vaFamily[assistantBehaviour - 1].transform;
    }

    public override void GenerateObjectsInWorld()
    {
        //Seleziono il pavimento
        //Transform floor = SpatialProcessing.Instance.floors.ElementAt(0).transform;
        //SurfacePlane plane = floor.GetComponent<SurfacePlane>();

        Transform anchorPosition = this.tableAnchor.transform;
        System.Random rnd = new System.Random();

        //Might need to "AdjustPositionWithSpatialMap"

        //Vector3 floorPosition = floor.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
        //floorPosition = AdjustPositionWithSpatialMap(floorPosition, plane.SurfaceNormal);

        /*
                Vector3 gazePosition = new Vector3(0f, 0f, 0f);
                RaycastHit hitInfo;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 20f, Physics.DefaultRaycastLayers))
                {
                    gazePosition = hitInfo.point;
                }

                Vector3 weatherPosition = gazePosition;
                weatherPosition.y = floorPosition.y + 1f;
                Debug.DrawLine(Camera.main.transform.position, weatherPosition, Color.black, 30f);
        */

        Vector3 weatherPosition = anchorPosition.position;
        weatherPosition.y = weatherPosition.y + 1f;

        Vector3 relativePos = Camera.main.transform.position;

        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = 0f;
        rotation.z = 0f;

        GameObject[] selectedLevel = numberOfLevel == 1 ? WheatherPrefabsLvl1 : WheatherPrefabsLvl2;
        GameObject selectedWeatherObj = selectedLevel[rnd.Next(0, selectedLevel.Length)];
        GameObject selectedWeather = PhotonNetwork.Instantiate(selectedWeatherObj.transform.name, weatherPosition, rotation);

        weathertag = selectedWeather.transform.GetChild(0).tag;
        temperaturetag = selectedWeather.transform.GetChild(1).tag;

        Debug.Log("Weather: " + weathertag);
        Debug.Log("Temperature: " + temperaturetag);

        Vector3 clothesPosition = anchorPosition.position;
        clothesPosition.y = clothesPosition.y + 0.1f;

        int counter = 0;
        for (int i = 0; i < numberOfClothes; i++)
        {
            Transform currentClothe = ClothesPrefabs[(rnd.Next(0, ClothesPrefabs.Length))].transform;
            List<string> tags = currentClothe.GetComponent<TagsContainer>().tags;
            currentClothe.tag = "ObjectsToBePlaced";

            Vector3 currentClothePosition = currentClothe.position + clothesPosition;

            if (counter <= Math.Floor((double)numberOfClothes / 3))
            {
                if (!tags.Contains(weathertag) && !tags.Contains(temperaturetag))
                {
                    i--;
                    continue;
                }
            }
            PhotonNetwork.Instantiate(currentClothe.name, currentClothePosition, currentClothe.rotation);

            if (tags.Contains(weathertag) || tags.Contains(temperaturetag))
            {
                counter++;
            }
        }
        Debug.Log("Number of clothes: " + numberOfClothes + ", correct clothes: " + counter);

        Vector3 bagPosition = weatherPosition;
        bagPosition.y = anchorPosition.position.y + 0.1f;

        PhotonNetwork.Instantiate(BagsPrefabs[(rnd.Next(0, BagsPrefabs.Length))].transform.name, bagPosition, rotation);

        //Debug.DrawLine(clothesPosition, bagPosition, Color.blue, 30f);

        Counter.Instance.InitializeCounter(counter);

        Vector3 assistantPosition = new Vector3(-0.3f, 0f, 0.3f) + bagPosition;
        assistantPosition.y = anchorPosition.position.y;

        if (assistantPresence != 0)
        {
            PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
        }
    }

    [PunRPC]
    public override void OnGameStarted()
    {
        //Set weather and temperature tag
        GameObject weatherObj = GameObject.FindGameObjectWithTag("Weather");

        weathertag = weatherObj.transform.GetChild(0).tag;
        temperaturetag = weatherObj.transform.GetChild(1).tag;
    }

    public override GameObject GetClosestObject()
    {
        GameObject[] remainingObjects = GameObject.FindGameObjectsWithTag("ObjectsToBePlaced");
        Debug.Log("Remaining Object: " + remainingObjects.Length);

        List<GameObject> targets = remainingObjects.Where(obj =>
        {
            List<string> tags = obj.GetComponent<TagsContainer>().tags;
            return tags.Contains(weathertag) || tags.Contains(temperaturetag);
        }).ToList();

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
}
