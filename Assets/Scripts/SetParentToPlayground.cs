using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentToPlayground : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        transform.parent = GameObject.Find("SharedPlayground").transform.GetChild(0).transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
