
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : Singleton<Counter> {

    private int count;

	// Use this for initialization
	void Start ()
    {
        
	}

    public void Decrement()
    {
        Debug.Log(count);
        count--;
        if (count == 0)
        {
            if (VirtualAssistantManager.Instance != null)
            {
                VirtualAssistantManager.Instance.GetComponent<Animator>().SetTrigger("EndGame");
            }
            Debug.Log("End Game");
        }
    }

    public void InitializeCounter(int count)
    {
        this.count = count;
    }

   
     
        

}
