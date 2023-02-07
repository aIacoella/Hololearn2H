

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

    public GameObject BinsPrefabs;
    public GameObject WastePrefabs;
    public GameObject VirtualAssistantsPrefabs;

    public GameObject BinsContainerPrefab;

    public GameObject WasteContainerPrefab;

    private int numberOfBins;
    private int numberOfWaste;
    private int assistantPresence;
    private int selectedAssistant;
    private int explainTaskGoal;
    private int assistantBehaviour;
    private int assistantPatience;

    private Transform virtualAssistant;

    public List<string> activeBins;

    // Use this for initialization
    public new void Start()
    {
        base.Start();

        cachePrefabs();

        LoadSettings();

        virtualAssistant = VirtualAssistantsPrefabs.transform.GetChild(selectedAssistant + 1).GetChild(assistantBehaviour - 1);
    }

    // Update is called once per frame
    public void Update()
    {

    }

    [PunRPC]
    public override void OnGameStarted()
    {
        Counter.Instance.InitializeCounter(GameObject.Find("Waste").GetComponentsInChildren<Rigidbody>().Length);
        activeBins = new List<string>();
        foreach (Transform bin in GameObject.Find("Bins").transform)
        {
            activeBins.Add(bin.tag);
            Debug.Log(bin.tag);
        }
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

        Vector3 binsPosition = anchorPosition.position;

        //binsPosition.y = floorPosition.y;

        //Transform sceneRoot = GameObject.Find("Broadcasted Content").transform;

        //Transform bins = new GameObject("Bins").transform;
        //bins.parent = sceneRoot;
        //bins.tag = "Targets";
        Transform bins = PhotonNetwork.Instantiate(BinsContainerPrefab.name, binsPosition, Quaternion.identity).transform;

        activeBins = new List<string>();
        for (int i = 1; i <= numberOfBins;)
        {
            Transform bin = BinsPrefabs.transform.GetChild(rnd.Next(0, BinsPrefabs.transform.childCount));
            string currentBinTag = bin.gameObject.tag;
            if (!activeBins.Contains(currentBinTag))
            {
                Vector3 currentBinPosition = binsPosition + new Vector3((float)Math.Pow(-1, i) * 0.4f * (i / 2), 0f, 0f);
                PhotonNetwork.Instantiate(bin.name, currentBinPosition, bin.rotation);
                activeBins.Add(bin.gameObject.tag);
                i++;
            }
        }

        //bins.Translate(binsPosition);
        //bins.Rotate(rotation.eulerAngles);

        //waste.parent = sceneRoot;
        //waste.tag = "ObjectsToBePlaced";

        //Vector3 wastePosition = Vector3.Lerp(Camera.main.transform.position, bins.position, 0.5f);
        //wastePosition.y = floorPosition.y + 0.1f;

        //Vector3 wastePosition = Vector3.Lerp(Camera.main.transform.position, bins.position, 0.5f);
        Vector3 wastePosition = bins.position;
        wastePosition.z = wastePosition.z - 1.0f;
        wastePosition.y = anchorPosition.position.y + 0.1f;

        Transform waste = PhotonNetwork.Instantiate(WasteContainerPrefab.name, wastePosition, Quaternion.identity).transform;

        for (int i = 0; i < numberOfWaste;)
        {
            Transform wasteGroup = WastePrefabs.transform.GetChild(rnd.Next(0, WastePrefabs.transform.childCount));
            int groupSize = wasteGroup.GetComponentsInChildren<Rigidbody>().Length;
            Transform currentWaste = wasteGroup.GetChild(rnd.Next(0, groupSize));
            string currentWasteTag = currentWaste.gameObject.tag;
            if (activeBins.Contains(currentWasteTag))
            {
                //Instantiate(currentWaste.gameObject, currentWaste.position, currentWaste.rotation, waste);
                PhotonNetwork.Instantiate(currentWaste.name, wastePosition + currentWaste.position, currentWaste.rotation);
                i++;
            }
        }

        //waste.Translate(wastePosition);
        //waste.Rotate(rotation.eulerAngles);

        Counter.Instance.InitializeCounter(waste.GetComponentsInChildren<Rigidbody>().Length);

        /*
        Vector3 assistantPosition = bins.TransformPoint(-0.3f, 0f, 0.3f);
        assistantPosition.y = floor.position.y;

        if (assistantPresence != 0)
        {
            Instantiate(virtualAssistant.gameObject, assistantPosition, virtualAssistant.transform.rotation, sceneRoot);
            VirtualAssistantManager.Instance.patience = assistantPatience;
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);

            if (explainTaskGoal == 1)
            {
                VirtualAssistantManager.Instance.ExplainTaskGoal();
            }
        }
        */

        Vector3 assistantPosition = new Vector3(-0.3f, 0f, -0.5f) + bins.position;
        //assistantPosition.y = anchorPosition.position.y;

        if (assistantPresence != 0)
        {
            PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            VirtualAssistantManager.Instance.patience = assistantPatience;
            //TODO: Find a solution for this one. (Maybe in OnGameStarted)
            VirtualAssistantManager.Instance.transform.localScale += new Vector3(0.25f * VirtualAssistantManager.Instance.transform.localScale.x, 0.25f * VirtualAssistantManager.Instance.transform.localScale.y, 0.25f * VirtualAssistantManager.Instance.transform.localScale.z);
            if (explainTaskGoal == 1)
            {
                VirtualAssistantManager.Instance.ExplainTaskGoal();
            }
        }
    }

    public override GameObject GetClosestTarget()
    {
        GameObject draggedObject = VirtualAssistantManager.Instance.targetObject.gameObject;
        string tag = draggedObject.tag;

        BoxCollider[] placements = GameObject.FindGameObjectWithTag("Targets").GetComponentsInChildren<BoxCollider>();
        List<GameObject> targets = new List<GameObject>();
        foreach (BoxCollider target in placements)
        {
            if (target.gameObject.tag == tag)
            {
                targets.Add(target.gameObject);
            }
        }

        SortByDistance(targets);
        return targets[0];
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


    private void cachePrefabs()
    {
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            Debug.Log("Caching all prefabs");

            if (BinsPrefabs != null)
            {
                foreach (Transform bin in BinsPrefabs.transform)
                {

                    if (!pool.ResourceCache.ContainsKey(bin.name))
                    {
                        pool.ResourceCache.Add(bin.name, bin.gameObject);
                    }

                }
            }

            if (WastePrefabs != null)
            {
                foreach (Transform wasteType in WastePrefabs.transform)
                {
                    foreach (Transform waste in wasteType)
                    {
                        if (!pool.ResourceCache.ContainsKey(waste.name))
                        {
                            pool.ResourceCache.Add(waste.name, waste.gameObject);
                        }
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

            if (!pool.ResourceCache.ContainsKey(BinsContainerPrefab.name))
            {
                pool.ResourceCache.Add(BinsContainerPrefab.name, BinsContainerPrefab);
            }

            if (!pool.ResourceCache.ContainsKey(WasteContainerPrefab.name))
            {
                pool.ResourceCache.Add(WasteContainerPrefab.name, WasteContainerPrefab);
            }

        }

    }
}
