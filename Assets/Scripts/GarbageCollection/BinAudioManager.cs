using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinAudioManager : MonoBehaviour
{
    public AudioClip clip;

    private AudioSource audioSource;
    public void Start()
    {
        this.audioSource = GetComponent<AudioSource>();
    }
    public void PlayBinSound()
    {
        this.audioSource.PlayOneShot(clip, 0.5f);
    }
}
