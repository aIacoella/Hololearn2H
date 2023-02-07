using Microsoft.MixedReality.SampleQRCodes;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace MRTK.Tutorials.MultiUserCapabilities
{
    public class TableAnchor : MonoBehaviour
    {
        public static TableAnchor Instance;

        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                if (Instance == this) return;
                Destroy(Instance.gameObject);
                Instance = this;
            }

            GetComponent<ARSessionOrigin>().camera = Camera.main;
        }
    }
}
