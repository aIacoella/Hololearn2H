

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class FindMeModeManager : PlayModeManager
{
    public Transform selectedElement;
    public Transform objectToFind;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void HandleTap(Transform selectedElement)
    {
        Debug.Log("HandleTap");
        IsBusy = true;

        GameObject box = selectedElement.GetChild(0).gameObject;
        GameObject item = selectedElement.GetChild(1).gameObject;

        box.SetActive(false);
        item.SetActive(true);
        item.transform.parent = selectedElement;

        //selectedElement.GetChild(0).gameObject.SetActive(false);
        //selectedElement.GetChild(1).gameObject.SetActive(true);

        this.selectedElement = selectedElement.GetChild(1);

        Transform confirmationMessage = transform.GetChild(1);
        confirmationMessage.position = selectedElement.position + new Vector3(0f, 0.3f, 0f);
        confirmationMessage.rotation = selectedElement.rotation;
        confirmationMessage.gameObject.SetActive(true);
    }


    public override List<GameObject> GenerateObjects(GameObject[] ObjectsPrefabs, int numberOfBoxes)
    {
        System.Random rnd = new System.Random();

        List<Transform> objs = new List<Transform>();
        for (int i = 1; i <= numberOfBoxes; i++)
        {
            //int j = rnd.Next(0, ObjectsPrefabs.transform.childCount);
            int j = rnd.Next(0, ObjectsPrefabs.Length);
            //Transform obj = ObjectsPrefabs.transform.GetChild(j);
            Transform obj = ObjectsPrefabs[j].transform;
            objs.Add(obj);
        }

        Counter.Instance.InitializeCounter(1);

        //return objs;

        return objs.Select(o => o.gameObject).ToList();
    }


    public override IEnumerator ShowObjects(int waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
        Debug.Log("ShowObjectToFind");
        System.Random rnd = new System.Random();

        Transform elems = GameObject.Find("Elements").transform;
        string objectToFind_string = elems.GetChild(rnd.Next(0, elems.childCount)).GetChild(1).name;
        //Instantiate(objectToFind, transform.GetChild(0).position + new Vector3(0f, -0.2f, 0f), transform.GetChild(0).rotation, transform.GetChild(0));
        GameObject obj = PhotonNetwork.Instantiate(objectToFind_string, transform.GetChild(0).position + new Vector3(0f, -0.2f, 0f), transform.GetChild(0).rotation, 0);

        //transform.GetChild(0).gameObject.SetActive(true);
        //transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

        yield return new WaitForSeconds(waitingTime);
        obj.name = objectToFind_string;
        objectToFind = obj.transform;

        obj.SetActive(false);

        //transform.GetChild(0).gameObject.SetActive(false);
    }


    public void YesConfirmation()
    {
        transform.GetChild(1).gameObject.SetActive(false);

        if (selectedElement.gameObject.name == objectToFind.gameObject.name)
        {
            if (VirtualAssistantManager.Instance != null)
            {
                VirtualAssistantManager.Instance.Jump();
            }
            Counter.Instance.Decrement();
        }
        else
        {
            if (VirtualAssistantManager.Instance != null)
            {
                VirtualAssistantManager.Instance.ShakeHead();
            }
            selectedElement.parent.GetChild(0).gameObject.SetActive(true);
            selectedElement.gameObject.SetActive(false);
        }

        IsBusy = false;
    }

    public void NoConfirmation()
    {
        transform.GetChild(1).gameObject.SetActive(false);

        selectedElement.parent.GetChild(0).gameObject.SetActive(true);
        selectedElement.gameObject.SetActive(false);

        selectedElement = null;

        IsBusy = false;
    }

    public override void InitCounter()
    {
        throw new NotImplementedException();
    }
}
