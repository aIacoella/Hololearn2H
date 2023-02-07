

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.IO;
using Photon.Pun;

//using Microsoft.MixedReality.Toolkit;

public class LayTheTableManager : RoomManager
{

    public GameObject LevelsPrefabs;
    public GameObject ObjectsPrefabs;
    public GameObject VirtualAssistantsPrefabs;
    public GameObject PlacementsManagerPrefabs;
    public GameObject TableMatsPrefab;
    public GameObject objectsToBePlacedPrefab;

    private int numberOfLevel;
    private int numberOfPeople;
    private int targetsVisibility;
    private int assistantPresence;
    private int selectedAssistant;
    private int assistantBehaviour;
    private int assistantPatience;

    private Transform virtualAssistant;
    private Transform selectedLevel;


    // Use this for initialization
    public new void Start()
    {
        base.Start();

        cachePrefabs();

        LoadSettings();

        selectedLevel = LevelsPrefabs.transform.GetChild(numberOfLevel - 1);
        virtualAssistant = VirtualAssistantsPrefabs.transform.GetChild(selectedAssistant + 1).GetChild(assistantBehaviour - 1);

        Instantiate(PlacementsManagerPrefabs.transform.GetChild(targetsVisibility), gameObject.transform);
    }

    public override void GenerateObjectsInWorld()
    {
        //Seleziono il tavolo dove guarda l'utente
        //Transform table = TableSelect(SpatialProcessing.Instance.tables);

        //Bounds tableColliderBounds = table.GetColliderBounds();

        Transform table = this.tableAnchor.transform;
        Vector3 tableCenter = table.position;

        Vector3 tableEdge1 = table.TransformPoint(0.4f, 0f, 0f);
        Vector3 tableEdge2 = table.TransformPoint(-0.4f, 0f, 0f);
        Vector3 tableEdge3 = table.TransformPoint(0f, 0f, -0.4f);
        Vector3 tableEdge4 = table.TransformPoint(0f, 0, 0.4f);


        List<Vector3> tableEdges = new List<Vector3>() { tableEdge1, tableEdge2, tableEdge3, tableEdge4 };
        Debug.DrawLine(tableEdge1, tableCenter, Color.black, 30f);
        Debug.DrawLine(tableEdge2, tableCenter, Color.black, 30f);
        Debug.DrawLine(tableEdge3, tableCenter, Color.red, 30f);
        Debug.DrawLine(tableEdge4, tableCenter, Color.red, 30f);

        List<Quaternion> rotations = new List<Quaternion>();

        for (int i = 0; i < tableEdges.Count; i++)
        {
            Vector3 relativeDirection = tableCenter - tableEdges.ElementAt(i);
            Quaternion rotation = Quaternion.LookRotation(relativeDirection);
            rotations.Add(rotation);
        }


        Transform objectsToBePlaced = selectedLevel.gameObject.GetComponent<ObjectsGenerator>().GenerateObjects(ObjectsPrefabs.transform, numberOfPeople, tableEdge1 + new Vector3(0, 0.3f, 0), rotations.ElementAt(0));
        //objectsToBePlaced.Translate(tableEdge1);
        //objectsToBePlaced.Rotate(rotations.ElementAt(0).eulerAngles);

        //Transform sceneRoot = GameObject.Find("Broadcasted Content").transform;

        //Transform tablePlacements = new GameObject("TableMates").transform;
        //tablePlacements.parent = sceneRoot;
        //tablePlacements.tag = "Targets";

        Transform tablePlacements = PhotonNetwork.Instantiate(TableMatsPrefab.name, Vector3.zero, Quaternion.identity).transform;

        Transform tableMatesPlacements = selectedLevel.Find("TableMatePlacementLV" + numberOfLevel);
        for (int i = 1; i <= numberOfPeople; i++)
        {
            //Instantiate(tableMatesPlacements.gameObject, tableEdges.ElementAt(i) + new Vector3(0f, 0.01f, 0f), rotations.ElementAt(i), tablePlacements);
            PhotonNetwork.Instantiate(tableMatesPlacements.name, tableEdges.ElementAt(i) + new Vector3(0f, 0.01f, 0f), rotations.ElementAt(i));
        }

        Transform beveragesPlacements = selectedLevel.Find("BeveragesPlacementLV" + numberOfLevel);
        //Instantiate(beveragesPlacements.gameObject, tableCenter + new Vector3(0f, 0.01f, 0f), beveragesPlacements.transform.rotation, tablePlacements);
        PhotonNetwork.Instantiate(beveragesPlacements.name, tableCenter + new Vector3(0f, 0.01f, 0f), beveragesPlacements.transform.rotation);

        Counter.Instance.InitializeCounter(objectsToBePlaced.GetComponentsInChildren<Rigidbody>().Length);

        Vector3 assistantPosition = table.TransformPoint(-0.2f, 0f, 0f);

        if (assistantPresence != 0)
        {
            //Instantiate(virtualAssistant.gameObject, assistantPosition, virtualAssistant.transform.rotation, sceneRoot);
            PhotonNetwork.Instantiate(virtualAssistant.name, assistantPosition, virtualAssistant.transform.rotation);

            //It should be ok to have it only assigned for the host as it is the one that controls it
            VirtualAssistantManager.Instance.patience = assistantPatience;
        }
    }




    private void LoadSettings()
    {
        numberOfPeople = LayTheTableSettings.Instance.numberOfPeople;
        numberOfLevel = LayTheTableSettings.Instance.numberOfLevel;
        targetsVisibility = LayTheTableSettings.Instance.targetsVisibility;
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
        Destroy(GameObject.Find("ObjectsToBePlaced"));
        Destroy(GameObject.Find("TableMates"));
    }

    [PunRPC]
    public override void OnGameStarted()
    {
        Counter.Instance.InitializeCounter(GameObject.Find("ObjectsToBePlaced").GetComponentsInChildren<Rigidbody>().Length);
    }

    private void cachePrefabs()
    {
        if (PhotonNetwork.PrefabPool is DefaultPool pool)
        {
            Debug.Log("Caching all prefabs");

            if (LevelsPrefabs != null)
            {
                foreach (Transform lvl in LevelsPrefabs.transform)
                {
                    foreach (Transform obj in lvl)
                    {
                        if (!pool.ResourceCache.ContainsKey(obj.name))
                        {
                            pool.ResourceCache.Add(obj.name, obj.gameObject);
                        }
                    }
                }
            }

            if (ObjectsPrefabs != null)
            {
                foreach (Transform objType in ObjectsPrefabs.transform)
                {
                    foreach (Transform obj in objType)
                    {
                        if (!pool.ResourceCache.ContainsKey(obj.name))
                        {
                            pool.ResourceCache.Add(obj.name, obj.gameObject);
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

            if (!pool.ResourceCache.ContainsKey(TableMatsPrefab.name))
            {
                pool.ResourceCache.Add(TableMatsPrefab.name, TableMatsPrefab);
            }

            if (!pool.ResourceCache.ContainsKey(objectsToBePlacedPrefab.name))
            {
                pool.ResourceCache.Add(objectsToBePlacedPrefab.name, objectsToBePlacedPrefab);
            }

        }

    }
}
