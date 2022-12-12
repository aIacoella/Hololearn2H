using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationEventHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void ManipulationStarted(GameObject draggedObject)
    {
        Debug.Log("Manipulation Started");
        if (VirtualAssistantManager.Instance != null)
        {
            VirtualAssistantManager.Instance.ObjectDragged(draggedObject);
        }
    }

    public void ManipulationStopped()
    {
        if (VirtualAssistantManager.Instance != null)
        {
            VirtualAssistantManager.Instance.ObjectDropped();
        }
    }
}
