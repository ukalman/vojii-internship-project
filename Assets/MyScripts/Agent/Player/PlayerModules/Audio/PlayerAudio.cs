using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioState
{
    Idle,
    Land,
    Jump,
    Dash,
    ThrowBomb
}

public class PlayerAudio : AgentModuleBase
{

    public List<AudioSource> PlayerSounds; //0th index is Landing sound effect
    public AudioState PlayerAudioState;

    //private void Awake()
    //{
    //    this.Priority = 1;
    //}
    public override IEnumerator IE_Initialize()
    {
        yield return StartCoroutine(base.IE_Initialize());
        this.moduleName = "Player Audio Module";
        this.Priority = 1;
        PlayerAudioState = AudioState.Idle;
    }

    public override IEnumerator IE_Activate()
    {
        yield return StartCoroutine(base.IE_Activate());

    }

    public override bool Tick()
    {
        if (base.Tick())
        {
            PlayAudio();
            return true;
        }

        return false;

    }

    public void PlayAudio()
    {
        switch (PlayerAudioState)
        {
            case AudioState.Idle:
                break;
            case AudioState.Land:
                PlayerSounds[0].Play();
                PlayerAudioState = AudioState.Idle;
                break;
            case AudioState.Jump:
                PlayerSounds[1].Play();
                PlayerAudioState = AudioState.Idle;
                break;
            case AudioState.Dash:
                PlayerSounds[2].Play();
                PlayerAudioState = AudioState.Idle;
                break;
            case AudioState.ThrowBomb:
                PlayerSounds[3].Play();
                PlayerAudioState = AudioState.Idle;
                break;
            default:
                break;
        }
    }

}
