

using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GarbageCollectionManager : RoomManager
{

    public GameObject[] BinsPrefabs;
    public GameObject[] WasteGlassPrefabs;
    public GameObject[] WastePaperPrefabs;
    public GameObject[] WastePlasticPrefabs;
    public GameObject[] MinionVirtualAssistantsPrefabs;
    public GameObject[] TYVirtualAssistantsPrefabs;
    public GameObject WastePrefab;


    private GameObject[][] WastePrefabs;
    private int numberOfBins;
    private int numberOfWaste;
    private int assistantPresence;
    private int selectedAssistant;
    private int explainTaskGoal;
    private int assistantBehaviour;
    private int assistantPatience;

    private Transform waste;

    private Transform virtualAssistant;

    public List<string> activeBins;

    // Use this for initialization
    public void Start()
    {
        base.Start();

        //initializing the array of arrays
        WastePrefabs = new GameObject[3][];

        // Allow prefabs not in a Resources folder
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            Debug.Log("Caching all prefabs");

            if (BinsPrefabs != null)
            {
                foreach (GameObject bin in BinsPrefabs)
                {
                    pool.ResourceCache.Add(bin.name, bin);
                }
            }

            if (WasteGlassPrefabs != null)
            {
                foreach (GameObject waste in WasteGlassPrefabs)
                {
                    pool.ResourceCache.Add(waste.name, waste);
                }
            }

            if (WastePaperPrefabs != null)
            {
                foreach (GameObject waste in WastePaperPrefabs)
                {
                    pool.ResourceCache.Add(waste.name, waste);
                }
            }

            if (WastePlasticPrefabs != null)
            {
                foreach (GameObject waste in WastePlasticPrefabs)
                {
                    pool.ResourceCache.Add(waste.name, waste);
                }
            }

            if (MinionVirtualAssistantsPrefabs != null)
            {
                foreach (GameObject assistant in MinionVirtualAssistantsPrefabs)
                {
                    pool.ResourceCache.Add(assistant.name, assistant);
                }
            }

            if (TYVirtualAssistantsPrefabs != null)
            {
                foreach (GameObject assistant in TYVirtualAssistantsPrefabs)
                {
                    pool.ResourceCache.Add(assistant.name, assistant);
                }
            }

            if (WastePrefab != null)
            {
                pool.ResourceCache.Add(WastePrefab.name, WastePrefab);
            }

            WastePrefabs[0] = WasteGlassPrefabs;
            WastePrefabs[1] = WastePaperPrefabs;
            WastePrefabs[2] = WastePlasticPrefabs;
        }

        LoadSettings();



        //virtualAssistant = VirtualAssistantsPrefabs.transform.GetChild(selectedAssistant + 1).GetChild(assistantBehaviour - 1);

        GameObject[] vaFamily = selectedAssistant == 0 ? MinionVirtualAssistantsPrefabs : TYVirtualAssistantsPrefabs;
        virtualAssistant = vaFamily[assistantBehaviour - 1].transform;


        //GameObject.Find("TaskMenu").GetComponent<TaskInteractionHandler>().OverrideAndStartPlaying();
    }

    // Update is called once per frame
    public void Update()
    {

    }

    [PunRPC]
    public override void OnGameStarted()
    {
    }


    public override void GenerateObjectsInWorld()
    {

        Debug.Log("Generating Objects in World");
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

        Vector3 binsPosition = gazePosition;
        binsPosition.y = floorPosition.y;


        Vector3 relativePos = Camera.main.transform.position - gazePosition;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = 0f;
        rotation.z = 0f;


        Transform sceneRoot = GameObject.Find("Broadcasted Content").transform;

        Transform bins = new GameObject("Bins").transform;
        bins.parent = sceneRoot;
        bins.tag = "Targets";

        activeBins = new List<string>();
        for (int i = 1; i <= numberOfBins;)
        {
            Transform bin = BinsPrefabs[rnd.Next(0, BinsPrefabs.Length)].transform;
            string currentBinTag = bin.gameObject.tag;
            if (!activeBins.Contains(currentBinTag))
            {
                //Instantiate(bin, new Vector3((float)Math.Pow(-1, i) * 0.4f * (i / 2), 0f, 0f), bin.rotation, bins);
                PhotonNetwork.Instantiate(bin.name, new Vector3((float)Math.Pow(-1, i) * 0.8f * (i / 2), 0f, 0f), bin.rotation, 0);
                activeBins.Add(bin.gameObject.tag);
                i++;
            }
        }

        bins.Translate(binsPosition);
        bins.Rotate(rotation.eulerAngles);


        //Transform waste = new GameObject("Waste").transform;
        Transform waste = PhotonNetwork.Instantiate(WastePrefab.name, Vector3.zero, Quaternion.identity).transform;
        //PhotonNetwork.Instantiate(WastePrefab.name, Vector3.zero, Quaternion.identity);
        //waste.name = "Waste";
        //waste.parent = sceneRoot;
        //waste.tag = "ObjectsToBePlaced";

        Vector3 wastePosition = Vector3.Lerp(Camera.main.transform.position, bins.position + new Vector3(0f, 5f, 0f), 0.5f);
        wastePosition.y = floorPosition.y + 0.1f;

        for (int i = 0; i < numberOfWaste;)
        {
            //Transform wasteGroup = WastePrefabs[rnd.Next(0, WastePrefabs.Length)].transform;
            int wasteGroupIndex = rnd.Next(0, WastePrefabs.Length);
            int groupSize = WastePrefabs[wasteGroupIndex].Length;
            //Transform currentWaste = wasteGroup.GetChild(rnd.Next(0, groupSize));
            GameObject currentWaste = WastePrefabs[wasteGroupIndex][rnd.Next(0, groupSize)];
            string currentWasteTag = currentWaste.gameObject.tag;
            if (activeBins.Contains(currentWasteTag))
            {
                //Instantiate(currentWaste.gameObject, currentWaste.position, currentWaste.rotation, waste);
                PhotonNetwork.Instantiate(currentWaste.name, currentWaste.transform.position, currentWaste.transform.rotation, 0);
                i++;
            }
        }

        //waste.Translate(wastePosition);
        //waste.Rotate(rotation.eulerAngles);


        //Counter.Instance.InitializeCounter(waste.GetComponentsInChildren<Rigidbody>().Length);


        Vector3 assistantPosition = bins.TransformPoint(-0.3f, 0f, 0.3f);
        assistantPosition.y = floor.position.y;

        /*if (assistantPresence != 0)
        {
            Instantiate(virtualAssistant.gameObject, assistantPosition, virtualAssistant.transform.rotation, sceneRoot);
            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);

            if (explainTaskGoal == 1)
            {
                VirtualAssistantManager.Instance.ExplainTaskGoal();
            }
        }*/

        if (assistantPresence != 0)
        {
            PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
        }

    }


    private void LoadSettings()
    {
        numberOfBins = GarbageCollectionSettings.Instance.numberOfBins;
        numberOfWaste = GarbageCollectionSettings.Instance.numberOfWaste;
        assistantPresence = VirtualAssistantChoice.Instance.assistantPresence;
        selectedAssistant = VirtualAssistantChoice.Instance.selectedAssistant;
        explainTaskGoal = VirtualAssistantSettings.Instance.explainTaskGoal;
        assistantBehaviour = VirtualAssistantSettings.Instance.assistantBehaviour;
        assistantPatience = VirtualAssistantSettings.Instance.assistantPatience;
    }


    public override void DestroyObjects()
    {
        if (VirtualAssistantManager.Instance != null)
        {
            Destroy(VirtualAssistantManager.Instance.gameObject);
        }
        Destroy(GameObject.Find("Bins"));
        Destroy(GameObject.Find("Waste"));
    }
}
