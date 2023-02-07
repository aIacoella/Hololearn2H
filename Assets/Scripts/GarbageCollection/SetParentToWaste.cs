using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentToWaste : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.parent = GameObject.Find("Waste").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
