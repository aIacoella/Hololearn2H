using System.Collections;
using Photon.Pun;
using UnityEngine;

public class BinCollisionManager : MonoBehaviour
{
    Vector3 floorPosition;

    public AudioClip clip;

    private AudioSource audioSource;

    private PhotonView photonView;
    // Use this for initialization
    void Start()
    {
        //floorPosition = GameObject.Find("SurfacePlane(Clone)").transform.position;
        //TODO: Remove This
        floorPosition = Vector3.zero;

        this.audioSource = GetComponent<AudioSource>();

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
        if (other.transform.parent && other.transform.parent.tag == "ObjectsToBePlaced")
        {
            photonView.RPC("remoteOnTriggerEnter", RpcTarget.All, other.transform.GetSiblingIndex());
        }
    }

    [PunRPC]
    void remoteOnTriggerEnter(int itemIndex)
    {
        GameObject waste = GameObject.FindGameObjectWithTag("ObjectsToBePlaced");
        GameObject item = waste.transform.GetChild(itemIndex).gameObject;

        if (item.CompareTag(gameObject.tag))
        {
            Counter.Instance.Decrement();

            //GetComponent<BinAudioManager>().PlayBinSound();
            audioSource.PlayOneShot(clip, 1);


            if (VirtualAssistantManager.Instance != null)
            {
                VirtualAssistantManager.Instance.Jump();
                VirtualAssistantManager.Instance.ObjectDropped();
            }

            item.transform.GetComponent<ObjectPositionManager>().HasCollided(transform);
        }
        else
        {
            GarbageCollectionManager manager = (GarbageCollectionManager)RoomManager.Instance;
            if (VirtualAssistantManager.Instance != null)
            {
                if (manager.activeBins.Contains(item.tag) && !VirtualAssistantManager.Instance.IsBusy)
                {
                    VirtualAssistantManager.Instance.ShakeHead();
                }
            }
        }
    }

}
