using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameModeSettingsManager : MonoBehaviour
{
    public PinchSlider numPlayerSlider;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setSceneToPlay(int sceneToPlay)
    {
        GameSettings.Instance.sceneToPlay = sceneToPlay;
    }

    public void setGameMode(bool isMultiplayer)
    {
        GameSettings.Instance.isMultiplayer = isMultiplayer;
        int startingNumPlayers = 1;
        if (isMultiplayer)
        {
            startingNumPlayers = 2;
        }
        GameSettings.Instance.numPlayer = startingNumPlayers;
        numPlayerSlider.transform.GetChild(1).GetChild(1).GetComponent<TextMesh>().text = $"{startingNumPlayers}";
    }

    public void updateNumPlayerSlider()
    {
        int value = Convert.ToInt32(numPlayerSlider.SliderValue * 8) + 2;
        numPlayerSlider.transform.GetChild(1).GetChild(1).GetComponent<TextMesh>().text = $"{value}";
    }

    public void setNumberOfPlayers()
    {
        int numPlayer = Convert.ToInt32(numPlayerSlider.SliderValue * 8) + 2;
        GameSettings.Instance.numPlayer = numPlayer;
    }

    public void loadScene()
    {
        SceneManager.LoadScene(GameSettings.Instance.sceneToPlay, LoadSceneMode.Additive);
    }

}
