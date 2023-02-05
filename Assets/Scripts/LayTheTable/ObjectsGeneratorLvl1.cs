using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ObjectsGeneratorLvl1 : ObjectsGenerator
{

    // Use this for initialization
    public override void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override Transform GenerateObjects(Transform objectsPrefab, int numberOfPeople, Vector3 otbpPosition, Quaternion otbpRotation)
    {
        GameObject objectsToBePlacedObj = PhotonNetwork.Instantiate(objectsToBePlacedPrefab.transform.name, otbpPosition, otbpRotation);
        Transform objectsToBePlaced = objectsToBePlacedObj.transform;

        Transform plates = objectsPrefab.Find("Plates");
        Transform plate = plates.GetChild(0);

        for (int i = 0; i < numberOfPeople; i++)
        {
            PhotonNetwork.Instantiate(plate.name, new Vector3(0.0f, 0.1f, 0.0f), plate.transform.rotation);
        }

        Transform glasses = objectsPrefab.Find("Glasses");
        Transform glassType = glasses.Find("Glass_2");
        for (int i = 0; i < numberOfPeople; i++)
        {
            PhotonNetwork.Instantiate(glassType.name, new Vector3(0.2f, 0.1f, 0.0f), glassType.transform.rotation);
            //Instantiate(glassType.gameObject, new Vector3(0.2f, 0.1f, 0.0f), glassType.transform.rotation, objectsToBePlaced);
        }

        Transform cutlery = objectsPrefab.Find("Cutlery");
        Transform cutleryType1 = cutlery.Find("Fork");
        Transform cutleryType2 = cutlery.Find("Tablespoon");
        for (int i = 0; i < numberOfPeople; i++)
        {
            //Instantiate(cutleryType1.gameObject, new Vector3(-0.3f, 0.01f, 0.0f), cutleryType1.transform.rotation, objectsToBePlaced);
            //Instantiate(cutleryType2.gameObject, new Vector3(-0.35f, 0.2f, 0.0f), cutleryType2.transform.rotation, objectsToBePlaced);

            PhotonNetwork.Instantiate(cutleryType1.name, new Vector3(-0.3f, 0.01f, 0.0f), cutleryType1.transform.rotation);
            PhotonNetwork.Instantiate(cutleryType2.name, new Vector3(-0.35f, 0.2f, 0.0f), cutleryType2.transform.rotation);
        }
        // per ora si usa una lattina- bisonga migliorare cn la bottiglia
        Transform beverages = objectsPrefab.Find("Beverages");
        Transform can = beverages.GetChild(0);

        //Instantiate(can.gameObject, new Vector3(-0.1f, 0.1f, 0.2f), can.transform.rotation, objectsToBePlaced);
        PhotonNetwork.Instantiate(can.name, new Vector3(-0.1f, 0.1f, 0.2f), can.transform.rotation);

        return objectsToBePlaced;
    }

}
