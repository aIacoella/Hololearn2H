

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ClassicModeManager : PlayModeManager
{
    public Transform firstElement;
    public Transform secondElement;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    [PunRPC]
    public void HandleTapRPC(int childNumber) {
        Transform selectedElement = GameObject.Find("Elements").transform.GetChild(childNumber).transform;
        
        GameObject box = selectedElement.GetChild(0).gameObject;
        GameObject item = selectedElement.GetChild(1).gameObject;

        box.SetActive(false);
        item.SetActive(true);
        item.transform.parent = selectedElement;
        // selectedElement.GetChild(0).gameObject.SetActive(false);
        // selectedElement.GetChild(1).gameObject.SetActive(true);

        if (firstElement != null)
        {
            IsBusy = true;

            secondElement = selectedElement.GetChild(1);
            if (firstElement.gameObject.name == secondElement.gameObject.name)
            {
                firstElement = null;
                secondElement = null;

                Counter.Instance.Decrement();
                //Counter.Instance.Decrement();

                if (VirtualAssistantManager.Instance != null)
                {
                    VirtualAssistantManager.Instance.Jump();
                }

                IsBusy = false;
            }
            else
            {
                IsBusy = true;

                if (VirtualAssistantManager.Instance != null)
                {
                    VirtualAssistantManager.Instance.ShakeHead();
                }

                StartCoroutine(Wait(selectedElement));
            }
        }
        else
        {
            firstElement = selectedElement.GetChild(1);
        }
    }

    public override void HandleTap(Transform selectedElement)
    {
        //child number of selected element
        int childNumber = selectedElement.GetSiblingIndex();

        this.photonView.RPC("HandleTapRPC", RpcTarget.All, childNumber);

    }


    private IEnumerator Wait(Transform selectedElement)
    {
        yield return new WaitForSeconds(3f);

        firstElement.parent.GetChild(0).gameObject.SetActive(true);
        firstElement.gameObject.SetActive(false);
        secondElement.parent.GetChild(0).gameObject.SetActive(true);
        secondElement.gameObject.SetActive(false);

        firstElement = null;
        secondElement = null;

        IsBusy = false;
    }


    public override List<GameObject> GenerateObjects(GameObject[] ObjectsPrefabs, int numberOfBoxes)
    {
        System.Random rnd = new System.Random();

        List<Transform> objs = new List<Transform>();
        List<string> createdObjs = new List<string>();

        List<GameObject> objects = new List<GameObject>();
        for (int i = 1; i <= numberOfBoxes / 2;)
        {
            //int j = rnd.Next(0, ObjectsPrefabs.transform.childCount);
            int j = rnd.Next(0, ObjectsPrefabs.Length);
            //Transform obj = ObjectsPrefabs.transform.GetChild(j);
            GameObject obj = ObjectsPrefabs[j];

            //if (!createdObjs.Contains(ObjectsPrefabs.transform.GetChild(j).name))
            if (!createdObjs.Contains(ObjectsPrefabs[j].name))
            {
                //objs.Add(obj);
                //objs.Add(obj);
                //createdObjs.Add(ObjectsPrefabs.transform.GetChild(j).name);
                createdObjs.Add(ObjectsPrefabs[j].name);
                objects.Add(obj);
                i++;
            }
        }

        //Counter.Instance.InitializeCounter(objs.Count);
        Counter.Instance.InitializeCounter(objects.Count);

        //return objs;

        //shuflle the game objects list
        List<GameObject> shuffledObjs = objects.OrderBy(x => rnd.Next()).ToList();

        return shuffledObjs;
    }


    public override void StartGame(int waitingTime)
    {
       ShowObjects(waitingTime);
    }

    private IEnumerator ShowObjects(int waitingTime)
    {
        Debug.Log("ShowObjects");
        Transform elems = GameObject.Find("Elements").transform;

        for (int i = 0; i < elems.childCount; i++)
        {
            elems.GetChild(i).GetChild(0).gameObject.SetActive(false);
            elems.GetChild(i).GetChild(1).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(waitingTime);

        for (int i = 0; i < elems.childCount; i++)
        {
            elems.GetChild(i).GetChild(0).gameObject.SetActive(true);
            elems.GetChild(i).GetChild(1).gameObject.SetActive(false);
        }
    }
}
