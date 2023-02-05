using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagCollisionManager : MonoBehaviour
{
    Vector3 floorPosition;

    // Use this for initialization
    void Start()
    {
        //floorPosition = GameObject.Find("SurfacePlane(Clone)").transform.position;
        //TODO: Remove This
        floorPosition = Vector3.zero;
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
        Debug.Log("On Trigger Enter");
        DressUpManager manager = (DressUpManager)DressUpManager.Instance;


        TagsContainer tagsContainer = other.transform.GetComponent<TagsContainer>();
        if (tagsContainer == null)
        {
            return;
        }
        List<string> tags = other.transform.GetComponent<TagsContainer>().tags;
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

                other.transform.GetComponent<ObjectPositionManager>().HasCollided(transform);
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
