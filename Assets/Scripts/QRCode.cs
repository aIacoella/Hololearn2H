// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Utilities;

public class QRCode : SingletonLight<QRCode>
{
    private Microsoft.MixedReality.QR.QRCode qrCode;

    private SpatialGraphNode node;

    public System.Guid Id { get; set; }

    public float PhysicalSize { get; private set; }
    public string CodeText { get; private set; }

    private long lastTimeStamp = 0;

    public bool isInitialized = false;


    // Use this for initialization
    private void initialize()
    {
        PhysicalSize = 0.1f;
        CodeText = "Dummy";
        if (qrCode == null)
        {
            throw new System.Exception("QR Code Empty");
        }

        PhysicalSize = qrCode.PhysicalSideLength;
        CodeText = qrCode.Data;
        lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;

        Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);

        node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
        Debug.Log("Initialize SpatialGraphNode Id= " + Id);

        gameObject.GetComponent<MeshRenderer>().enabled = true;

        isInitialized = true;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (qrCode != null)
        {
            if (node == null || node.Id != Id)
            {
                initialize();
            }

            if (lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                PhysicalSize = qrCode.PhysicalSideLength;
                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
            }
        }

        if (node != null)
        {
            if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                // If there is a parent to the camera that means we are using teleport and we should not apply the teleport
                // to these objects so apply the inverse
                if (CameraCache.Main.transform.parent != null)
                {
                    pose = pose.GetTransformedBy(CameraCache.Main.transform.parent);
                }

                gameObject.transform.rotation = pose.rotation;
                gameObject.transform.position = pose.position - new Vector3(PhysicalSize / 2.0f, PhysicalSize / 2.0f, 0.0f);
                gameObject.transform.localScale = new Vector3(PhysicalSize, PhysicalSize, 0.005f);

                Debug.Log("Id= " + Id + " QRPose = " + pose.position.ToString("F7") + " QRRot = " + pose.rotation.ToString("F7"));
            }
            else
            {
                Debug.LogWarning("Cannot locate " + Id);
            }
        }

    }

    public void setQrCode(Microsoft.MixedReality.QR.QRCode qr)
    {
        this.qrCode = qr;
        this.Id = qr.SpatialGraphNodeId;
    }


}

