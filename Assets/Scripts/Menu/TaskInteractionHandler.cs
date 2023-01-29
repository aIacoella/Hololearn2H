
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Parallel [
    {
        1) Single Player or Multiplayer
        2) If Multiplayer, how many players? (2-10)
    }
]

*/

public class TaskInteractionHandler : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealitySourceStateHandler
{

    private bool readyToPlay;
    private bool playing;
    private bool handRecognized;

    // Use this for initialization
    void Start()
    {
        readyToPlay = false;
        playing = false;
        handRecognized = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnSourceDetected(SourceStateEventData eventData)
    {
        Debug.Log("SourceDetected");
        if (playing)
        {
            if (!handRecognized)
            {
                handRecognized = true;
                StartCoroutine(WaitForSecondSource());
            }
            else
            {
                SetMenuVisible();
            }
        }
    }

    private IEnumerator WaitForSecondSource()
    {
        yield return new WaitForSeconds(0.1f);
        handRecognized = false;
    }


    public void OnSourceLost(SourceStateEventData eventData)
    {
        handRecognized = false;
    }


    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        //throw new System.NotImplementedException();
    }


    public void SetMenuVisible()
    {
        playing = false;
        transform.GetChild(2).gameObject.SetActive(true);
    }

    public void SetMenuInvisible()
    {
        playing = true;
        transform.GetChild(2).gameObject.SetActive(false);
    }

    public void ScanningComplete()
    {
        Debug.Log("Scanning Complete");
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        StartCoroutine(Wait());
    }

    public void StartPlay()
    {

        Debug.Log("StartPlay");
        if (readyToPlay)
        {
            playing = true;
            TaskManager.Instance.GenerateObjectsInWorld();
            readyToPlay = false;
        }
    }

    private IEnumerator Wait()
    {

        Debug.Log("Wait");
        yield return new WaitForSeconds(3f);
        readyToPlay = true;
        transform.GetChild(1).gameObject.SetActive(false);
    }


    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Pointer Clicked: " + readyToPlay);
        if (readyToPlay)
        {
            StartPlay();
        }
    }

    public void OverrideAndStartPlaying()
    {
        Debug.Log("Is Playing: " + playing);

        readyToPlay = true;
        handRecognized = true;
        ScanningComplete();
        StartPlay();
    }
}
