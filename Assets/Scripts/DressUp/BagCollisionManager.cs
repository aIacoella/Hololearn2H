using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BagCollisionManager : MonoBehaviour
{
    Vector3 floorPosition;

    // Use this for initialization

    private PhotonView photonView;

    void Start()
    {
        //floorPosition = GameObject.Find("SurfacePlane(Clone)").transform.position;
        //TODO: Remove This
        floorPosition = GameObject.Find("TableAnchor").transform.position;

        this.photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < floorPosition.y)
        {
            //transform.position = new Vector3(transform.position.x, floorPosition.y + 0.01f, transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (photonView && other.transform.parent && other.transform.parent.tag == "ObjectsToBePlaced")
        {
            photonView.RPC("remoteOnTriggerEnter", RpcTarget.All, other.transform.GetSiblingIndex());
        }
    }


    [PunRPC]
    void remoteOnTriggerEnter(int itemIndex)
    {
        Debug.Log("On Trigger Enter");
        DressUpManager manager = (DressUpManager)DressUpManager.Instance;

        GameObject clothes = GameObject.FindGameObjectWithTag("ObjectsToBePlaced");
        GameObject item = clothes.transform.GetChild(itemIndex).gameObject;


        TagsContainer tagsContainer = clothes.transform.GetComponent<TagsContainer>();
        if (tagsContainer == null)
        {
            return;
        }
        List<string> tags = item.transform.GetComponent<TagsContainer>().tags;
        string weather = manager.getWeather();
        string temperature = manager.getTemerature();

        foreach (string tag in tags)
        {
            if (tags.Contains(weather) || tags.Contains(temperature))
            {
                Counter.Instance.Decrement();

                if (VirtualAssistantManager.Instance != null)
                {
                    VirtualAssistantManager.Instance.Jump();
                    VirtualAssistantManager.Instance.ObjectDropped();
                }

                item.transform.GetComponent<ObjectPositionManager>().HasCollided(transform);
            }
            else
            {
                if (VirtualAssistantManager.Instance != null && !VirtualAssistantManager.Instance.IsBusy)
                {
                    VirtualAssistantManager.Instance.ShakeHead();
                }
            }
        }


    }

}
