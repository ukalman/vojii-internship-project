using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : EnvironmentModuleBase
{
    // Bu arkada� hovering platformlar� tutacak
    public List<GameObject> HoveringPlatforms;


    public override IEnumerator IE_Initialize()
    {
        yield return StartCoroutine(base.IE_Initialize());
        HoveringPlatforms = new List<GameObject>(GameObject.FindGameObjectsWithTag("HoveringPlatform"));

        for (int i = 0; i < HoveringPlatforms.Count; i++)
        {
            HoveringPlatforms[i].GetComponent<PlatformWaveMovement>().Initialize();
        }

    }

    public override bool Tick()
    {
        if (base.Tick())
        {
            for (int i = 0; i < HoveringPlatforms.Count; i++)
            {
                HoveringPlatforms[i].GetComponent<PlatformWaveMovement>().Tick();
            }

            return true;
        }

        return false;



    }

    public override bool FixedTick()
    {
        if (base.FixedTick())
        {
            for (int i = 0; i < HoveringPlatforms.Count; i++)
            {
                HoveringPlatforms[i].GetComponent<PlatformWaveMovement>().FixedTick();
            }

            return true;
        }

        return false;

    }
}
