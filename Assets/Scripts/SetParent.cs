using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParent : MonoBehaviour
{
    public string ParentName;

    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = GameObject.Find(ParentName);
        if (parent != null)
        {
            transform.parent = parent.transform;
        }
        else
        {
            throw new System.Exception("Can't set parent to null object");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
