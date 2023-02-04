// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.SampleQRCodes
{
    [RequireComponent(typeof(SpatialGraphNodeTracker))]
    public class QRCode : Singleton<QRCode>, IMixedRealityPointerHandler
    {
        public Microsoft.MixedReality.QR.QRCode qrCode;
        public float PhysicalSize { get; private set; }
        public string CodeText { get; private set; }

        private long lastTimeStamp = 0;

        public bool isInitialized = false;

        public bool hasBeenLocated = false;

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

            Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " QRVersion = " + qrCode.Version + " QRData = " + CodeText);

            isInitialized = true;
        }

        void Start()
        {
        }

        void UpdatePropertiesDisplay()
        {
            // Update properties that change
            if (qrCode != null && lastTimeStamp != qrCode.SystemRelativeLastDetectedTime.Ticks)
            {
                PhysicalSize = qrCode.PhysicalSideLength;
                Debug.Log("Id= " + qrCode.Id + "NodeId= " + qrCode.SpatialGraphNodeId + " PhysicalSize = " + PhysicalSize + " TimeStamp = " + qrCode.SystemRelativeLastDetectedTime.Ticks + " Time = " + qrCode.LastDetectedTime.ToString("MM/dd/yyyy HH:mm:ss.fff"));

                lastTimeStamp = qrCode.SystemRelativeLastDetectedTime.Ticks;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (qrCode != null)
            {
                if (!isInitialized)
                {
                    initialize();
                }
                UpdatePropertiesDisplay();
            }
        }

        void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }

        void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            // eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}
