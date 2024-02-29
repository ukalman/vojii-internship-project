using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EffectState
{
    Idle,
    Land,
    Attack
}

public class PlayerEffects : AgentModuleBase
{
    public EffectState PlayerEffectState;

    // Landing Effect Particle System
    public ParticleSystem LandingEffect;
    private short landingMinBurstCount;
    private short landingMaxBurstCount;
    public Vector3 LandingImpulse;

    public override IEnumerator IE_Initialize()
    {
        yield return StartCoroutine(base.IE_Initialize());
        PlayerEffectState = EffectState.Idle;

        landingMinBurstCount = 12;
        landingMaxBurstCount = 12;
        LandingImpulse = Vector3.zero;
    }

    public override IEnumerator IE_Activate()
    {
        yield return StartCoroutine(base.IE_Activate());
    }

    public override bool Tick()
    {
        
        return base.Tick();

    }

    public override bool FixedTick()
    {
        if (base.FixedTick())
        {
            PlayEffect();
            return true;
        }

        return false;

    }


    public void PlayEffect()
    {
        switch (PlayerEffectState)
        {
            case EffectState.Idle:
                break;
            case EffectState.Land:
                PlayLandingEffect();

                break;
            case EffectState.Attack:
                break;



            default:
                break;
        }
    }


    void PlayLandingEffect()
    {
        if (!LandingEffect.isPlaying)
        {
            var emission = LandingEffect.emission;
            var bursts = new ParticleSystem.Burst[emission.burstCount];
            emission.GetBursts(bursts);

            bursts[0].minCount = (short)(landingMinBurstCount * LandingImpulse.y);
            bursts[0].maxCount = (short)(landingMaxBurstCount * LandingImpulse.y);


            emission.SetBursts(bursts);

            LandingEffect.Play();
        }

        PlayerEffectState = EffectState.Idle;
    }

}
