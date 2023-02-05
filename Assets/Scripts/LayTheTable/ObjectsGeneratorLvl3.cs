﻿using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ObjectsGeneratorLvl3 : ObjectsGenerator
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

        // genera i piatti di colore random
        Transform plates = objectsPrefab.Find("Plates");
        Transform plate = plates.GetChild(rnd.Next(0, plates.childCount - 1));

        for (int i = 0; i < numberOfPeople; i++)
        {
            PhotonNetwork.Instantiate(plate.name, new Vector3(0.0f, 0.1f, 0.0f), plate.transform.rotation);
        }

        //genera i bicchieri random  di colore random
        Transform glasses = objectsPrefab.Find("Glasses");
        Transform glassType = glasses.GetChild(rnd.Next(0, glasses.childCount));
        Vector3 glassPosition = new Vector3(0.2f, 0.1f, 0.0f);

        for (int i = 0; i < numberOfPeople; i++)
        {
            //Instantiate(glassType.gameObject, glassPosition + new Vector3(0.1f, 0f, 0f), glassType.transform.rotation, objectsToBePlaced);
            PhotonNetwork.Instantiate(glassType.name, glassPosition + new Vector3(0.1f, 0f, 0f), glassType.transform.rotation);
        }

        Transform cutlery = objectsPrefab.Find("Cutlery");
        Transform cutleryType1 = cutlery.GetChild(rnd.Next(4, 8));
        Transform cutleryType2 = cutlery.GetChild(rnd.Next(8, 12));
        Transform cutleryType3 = cutlery.GetChild(rnd.Next(12, 16));

        for (int i = 0; i < numberOfPeople; i++)
        {
            //Instantiate(cutleryType1.gameObject, new Vector3(-0.3f, 0.01f, 0.0f), cutleryType1.transform.rotation, objectsToBePlaced);
            //Instantiate(cutleryType2.gameObject, new Vector3(-0.35f, 0.2f, 0.0f), cutleryType2.transform.rotation, objectsToBePlaced);
            //Instantiate(cutleryType3.gameObject, new Vector3(-0.4f, 0.3f, 0.0f), cutleryType3.transform.rotation, objectsToBePlaced);

            PhotonNetwork.Instantiate(cutleryType1.name, new Vector3(-0.3f, 0.01f, 0.0f), cutleryType1.transform.rotation);
            PhotonNetwork.Instantiate(cutleryType2.name, new Vector3(-0.35f, 0.2f, 0.0f), cutleryType2.transform.rotation);
            PhotonNetwork.Instantiate(cutleryType3.name, new Vector3(-0.4f, 0.3f, 0.0f), cutleryType3.transform.rotation);
        }


        Transform beverages = objectsPrefab.Find("Beverages");
        Transform bottle = beverages.GetChild(0);
        //Instantiate(bottle.gameObject, new Vector3(0.1f, 0.1f, 0.2f), bottle.transform.rotation, objectsToBePlaced);
        PhotonNetwork.Instantiate(bottle.name, new Vector3(0.1f, 0.1f, 0.2f), bottle.transform.rotation);


        Transform can1 = beverages.GetChild(rnd.Next(1, 3));
        Transform can2 = beverages.GetChild(rnd.Next(1, 3));

        //Instantiate(can1.gameObject, new Vector3(-0.05f, 0.1f, 0.2f), can1.transform.rotation, objectsToBePlaced);
        //Instantiate(can2.gameObject, new Vector3(-0.15f, 0.15f, 0.2f), can2.transform.rotation, objectsToBePlaced);
        PhotonNetwork.Instantiate(can1.name, new Vector3(-0.05f, 0.1f, 0.2f), can1.transform.rotation);
        PhotonNetwork.Instantiate(can2.name, new Vector3(-0.15f, 0.15f, 0.2f), can2.transform.rotation);

        return objectsToBePlaced;
    }



}
