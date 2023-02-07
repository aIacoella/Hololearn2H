using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class PlayModeManager : MonoBehaviourPun
{
    public bool IsBusy;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public abstract List<GameObject> GenerateObjects(GameObject[] ObjectsPrefabs, int numberOfBoxes);

    public abstract IEnumerator ShowObjects(int waitingTime);

    public abstract void HandleTap(Transform parent);

    public abstract void InitCounter();
}
