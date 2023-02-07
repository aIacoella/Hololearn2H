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

    public int GenerateTemperature()
    {
        return new System.Random().Next(MinRange, MaxRange);
    }

    public void DisplayTemperature()
    {
        Transform weather = transform.parent.parent;
        DressUpManager manager = (DressUpManager)RoomManager.Instance;

        Vector3 weatherPosition = weather.position;
        Vector3 temperaturePosition = weatherPosition;
        Vector3 relativePos = weatherPosition - Camera.main.transform.position;
        Quaternion temperatureRotation = Quaternion.LookRotation(relativePos);
        temperatureRotation.x = 0f;
        temperatureRotation.z = 0f;

        temperaturePosition += (Vector3.Normalize(temperatureRotation.eulerAngles) * 0.3f);

        Transform temperature = Instantiate(manager.WeatherPrefabs.transform.GetChild(0), temperaturePosition, temperatureRotation, weather.GetChild(0).GetChild(1));

        temperature.GetComponent<TextMesh>().text = manager.getTemperatureValue() + "°C";
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
