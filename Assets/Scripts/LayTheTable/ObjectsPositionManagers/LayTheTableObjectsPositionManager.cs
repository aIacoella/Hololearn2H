using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LayTheTableObjectsPositionManager : ObjectPositionManager
{

    private float lerpPercentage;
    private bool hasCollided;

    protected Vector3 finalPosition;
    protected Quaternion finalRotation;
    private Vector3 floorPosition;

    private Rigidbody rigidbody;

    // Use this for initialization
    public override void Start()
    {
        hasCollided = false;
        floorPosition = GameObject.Find("TableAnchor").transform.position;
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void Update()
    {
        if (Mathf.Abs(transform.position.y - floorPosition.y) > 5)
        {
            transform.position = new Vector3(transform.position.x, floorPosition.y + 0.3f, transform.position.z);
            rigidbody.velocity = Vector3.zero;
        }
    }

    public override void HasCollided(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        AdjustTransform();
    }

    public virtual void AdjustTransform()
    {
        return;
    }
}
