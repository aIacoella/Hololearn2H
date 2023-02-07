using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeNameOnStartup : MonoBehaviour
{
    public string NameToBe;

    // Start is called before the first frame update
    void Awake()
    {
        gameObject.name = NameToBe;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
