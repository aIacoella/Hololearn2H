using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TemperatureGenerator : MonoBehaviour
{

    public int MinRange;
    public int MaxRange;

    // Use this for initialization
    void Start()
    {
        this.GenerateTemperature();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateTemperature()
    {
        Transform weather = gameObject.transform.parent;
        DressUpManager manager = (DressUpManager)DressUpManager.Instance;

        Vector3 temperaturePostion = weather.TransformPoint(0.4f, 0f, 0f);
        Vector3 relativePos = temperaturePostion - Camera.main.transform.position;
        Quaternion temperatureRotation = Quaternion.LookRotation(relativePos);
        temperatureRotation.x = 0f;
        temperatureRotation.z = 0f;

        GameObject temperature = PhotonNetwork.Instantiate(manager.TemperatureTextPrefab.name, temperaturePostion, temperatureRotation, 0);

        int temperatureValue = new System.Random().Next(MinRange, MaxRange);
        temperature.GetComponent<TextMesh>().text = temperatureValue + "°C";

        /*int unit = temperature % 10;
        int dec = (temperature - unit) / 10;

        if (dec != 0)
        {
            Instantiate(manager.WeatherPrefabs.transform.GetChild(0).GetChild(dec), temperaturePostion, temperatureRotation, weather.GetChild(0).GetChild(1));
        }
        Instantiate(manager.WeatherPrefabs.transform.GetChild(0).GetChild(unit), temperaturePostion + new Vector3(0.1f, 0f, 0f), temperatureRotation, weather.GetChild(0).GetChild(1));
        Instantiate(manager.WeatherPrefabs.transform.GetChild(0).GetChild(manager.WeatherPrefabs.transform.GetChild(0).childCount - 2), temperaturePostion + new Vector3(0.2f, 0f, 0f), temperatureRotation, weather.GetChild(0).GetChild(1));
        Instantiate(manager.WeatherPrefabs.transform.GetChild(0).GetChild(manager.WeatherPrefabs.transform.GetChild(0).childCount - 1), temperaturePostion + new Vector3(0.3f, 0f, 0f), temperatureRotation, weather.GetChild(0).GetChild(1));*/
    }
}
