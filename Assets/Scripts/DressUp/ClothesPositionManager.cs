using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesPositionManager : ObjectPositionManager
{

    private bool hasCollided;
    private Vector3 targetPosition;
    private Vector3 floorPosition;
    private Rigidbody rigidbody;

    // Use this for initialization
    public override void Start()
    {
        hasCollided = false;
        targetPosition = new Vector3();
        floorPosition = GameObject.Find("TableAnchor").transform.position;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public override void Update()
    {
        if (Mathf.Abs(transform.position.y - floorPosition.y) > 5)
        {
            transform.position = new Vector3(transform.position.x, floorPosition.y + 1f, transform.position.z);
            rigidbody.velocity = Vector3.zero;
        }

        if (hasCollided)
        {
            //Debug.Log("Has Collided Update");
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
            transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale / 5, Time.deltaTime * 5f);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                gameObject.SetActive(false);
            }
        }
    }


    public override void HasCollided(Transform target)
    {
        transform.GetComponent<ObjectManipulator>().enabled = false;
        transform.GetComponent<Collider>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;

        targetPosition = target.position;

        hasCollided = true;
    }
}
