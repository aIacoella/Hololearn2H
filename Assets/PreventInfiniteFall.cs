using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventInfiniteFall : MonoBehaviour
{

    public GameObject player;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Transform>().position = new Vector3(player.transform.position.x, player.transform.position.y - 100, player.transform.position.z);
    }

    void onCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.position = new Vector3(collision.gameObject.transform.position.x, player.transform.position.y, collision.gameObject.transform.position.z);
    }
}
