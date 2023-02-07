using System.Collections;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using UnityEngine;

public class PlacementCollisionManager : MonoBehaviour
{

    private PhotonView photonView;

    // Use this for initialization
    void Start()
    {
        this.photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

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
        GameObject objectsToBePlaced = GameObject.FindGameObjectWithTag("ObjectsToBePlaced");
        GameObject item = objectsToBePlaced.transform.GetChild(itemIndex).gameObject;

        if (item.gameObject.CompareTag(gameObject.tag))
        {
            item.gameObject.GetComponent<ObjectManipulator>().enabled = false;

            Counter.Instance.Decrement();

            if (VirtualAssistantManager.Instance != null)
            {
                VirtualAssistantManager.Instance.Jump();
                VirtualAssistantManager.Instance.ObjectDropped();
            }

            if (item.gameObject.GetComponent<ObjectPositionManager>() != null)
            {
                item.gameObject.GetComponent<ObjectPositionManager>().HasCollided(transform);
            }

            Destroy(gameObject);
        }
        else
        {
            if (item.gameObject.tag != "Untagged" && !VirtualAssistantManager.Instance.IsBusy)
            {
                VirtualAssistantManager.Instance.ShakeHead();
            }
        }
    }

}
