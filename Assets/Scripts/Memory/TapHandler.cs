using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapHandler : MonoBehaviour, IMixedRealityPointerHandler
{
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //PlayModeManager manager = GameObject.Find("TaskLobby").transform.GetChild(0).transform.GetChild(0).GetComponent<PlayModeManager>();
        //get the memory manager
        MemoryManager memoryManager = GameObject.Find("TaskLobby").transform.GetChild(0).GetComponent<MemoryManager>();
        int playMode = memoryManager.GetPlayMode();
        PlayModeManager manager = GameObject.Find("TaskLobby").transform.GetChild(0).transform.GetChild(0).transform.GetChild(playMode).GetComponent<PlayModeManager>();
        if (!manager.IsBusy)
        {
            //GameObject.Find("TaskLobby").transform.GetChild(0).transform.GetChild(0).GetComponent<PlayModeManager>().HandleTap(transform.parent);
            manager.HandleTap(transform.parent);
        }
    }
}
