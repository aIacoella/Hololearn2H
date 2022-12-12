
using UnityEngine;

public class MinionAudioManager : AssistantAudioManagerInterface
{
    public AudioClip ShakingHeadNo;
    public AudioClip Jump;


    public override void PlayShakingHeadNo()
    {
        GetComponent<AudioSource>().PlayOneShot(ShakingHeadNo, 1);
    }

    public override void PlayJump()
    {
        GetComponent<AudioSource>().PlayOneShot(Jump, 1);
    }


    public override void PlayWalking()
    {

    }

    public override void PlayIntro()
    {

    }

    public override void PlayPointing()
    {

    }

}