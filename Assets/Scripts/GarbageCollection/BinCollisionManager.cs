using System.Collections;
using UnityEngine;

public class BinCollisionManager : MonoBehaviour
{
    Vector3 floorPosition;

    public AudioClip clip;

    private AudioSource audioSource;
    // Use this for initialization
    void Start()
    {
        //floorPosition = GameObject.Find("SurfacePlane(Clone)").transform.position;
        //TODO: Remove This
        floorPosition = Vector3.zero;

        this.audioSource = GetComponent<AudioSource>();
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
        if (other.gameObject.CompareTag(gameObject.tag))
        {
            Counter.Instance.Decrement();

            //GetComponent<BinAudioManager>().PlayBinSound();
            audioSource.PlayOneShot(clip, 1);


            if (VirtualAssistantManager.Instance != null)
            {
                VirtualAssistantManager.Instance.Jump();
                VirtualAssistantManager.Instance.ObjectDropped();
            }

            other.transform.GetComponent<ObjectPositionManager>().HasCollided(transform);
        }
        else
        {
            GarbageCollectionManager manager = (GarbageCollectionManager)RoomManager.Instance;
            if (VirtualAssistantManager.Instance != null)
            {
                if (manager.activeBins.Contains(other.tag) && !VirtualAssistantManager.Instance.IsBusy)
                {
                    VirtualAssistantManager.Instance.ShakeHead();
                }
            }
        }
    }

}
