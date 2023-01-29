
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : Singleton<GameSettings>
{
    public int sceneToPlay;
    public bool isMultiplayer;
    public int numPlayer;

    public string getSceneName()
    {
        return "ROOM_NAME";
    }
}
