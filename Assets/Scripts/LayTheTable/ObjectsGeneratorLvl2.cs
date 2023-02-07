using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ObjectsGeneratorLvl2 : ObjectsGenerator
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
        System.Random rnd = new System.Random();

        GameObject objectsToBePlacedObj = PhotonNetwork.Instantiate(objectsToBePlacedPrefab.transform.name, Vector3.zero, Quaternion.identity);
        Transform objectsToBePlaced = objectsToBePlacedObj.transform;

        Transform plates = objectsPrefab.Find("Plates");
        Transform plate = plates.GetChild(0);

        for (int i = 0; i < numberOfPeople; i++)
        {
            PhotonNetwork.Instantiate(plate.name, new Vector3(0.0f, 0.1f, 0.0f), plate.transform.rotation);
        }

        Transform glasses = objectsPrefab.Find("Glasses");
        Transform glassType = glasses.GetChild(rnd.Next(0, 6));
        Vector3 glassPosition = new Vector3(0.2f, 0.1f, 0.0f);
        for (int i = 0; i < numberOfPeople; i++)
        {
            //Instantiate(glassType.gameObject, glassPosition + new Vector3(0.1f, 0f, 0f), glassType.transform.rotation, objectsToBePlaced);
            PhotonNetwork.Instantiate(glassType.name, glassPosition + new Vector3(0.1f, 0f, 0f), glassType.transform.rotation);
        }

        Transform cutlery = objectsPrefab.Find("Cutlery");
        Transform cutleryType1 = cutlery.Find("Fork");
        Transform cutleryType2 = cutlery.GetChild(rnd.Next(1, 3));
        for (int i = 0; i < numberOfPeople; i++)
        {
            //Instantiate(cutleryType1.gameObject, new Vector3(-0.3f, 0.01f, 0.0f), cutleryType1.transform.rotation, objectsToBePlaced);
            //Instantiate(cutleryType2.gameObject, new Vector3(-0.35f, 0.2f, 0.0f), cutleryType2.transform.rotation, objectsToBePlaced);

            PhotonNetwork.Instantiate(cutleryType1.name, new Vector3(-0.3f, 0.01f, 0.0f), cutleryType1.transform.rotation);
            PhotonNetwork.Instantiate(cutleryType2.name, new Vector3(-0.35f, 0.2f, 0.0f), cutleryType2.transform.rotation);
        }

        Transform beverages = objectsPrefab.Find("Beverages");
        Transform bottle = beverages.GetChild(0);
        //Instantiate(bottle.gameObject, new Vector3(0.1f, 0.1f, 0.2f), bottle.transform.rotation, objectsToBePlaced);
        PhotonNetwork.Instantiate(bottle.name, new Vector3(0.1f, 0.1f, 0.2f), bottle.transform.rotation);

        Transform can = beverages.GetChild(rnd.Next(1, 3));
        //Instantiate(can.gameObject, new Vector3(-0.1f, 0.1f, 0.2f), can.transform.rotation, objectsToBePlaced);
        PhotonNetwork.Instantiate(can.name, new Vector3(-0.1f, 0.1f, 0.2f), can.transform.rotation);

        return objectsToBePlaced;
    }
}
