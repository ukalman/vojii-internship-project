using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleState
{
    Uninitialized,
    Initialized,
    PostInitialized,
    Activated,
    Deactivated,
    Disabled,
    Paused,
    Resumed,
    Restarted
}

public interface IModule
{
    public IModularObject Parent { get; set; }
    public ModuleState State {get; set;}
    public int Priority { get; set; }

    IEnumerator IE_SetState(ModuleState state);

    IModularObject GetParent();

    IEnumerator SetParent(IModularObject Parent);

    string GetName();

    IEnumerator IE_Deinitialize(); 
    IEnumerator IE_Initialize();
    IEnumerator IE_PostInitialize();
    IEnumerator IE_Activate();
    void Deactivate();
    IEnumerator IE_Disable();
    IEnumerator IE_Pause();
    IEnumerator IE_Resume();
    IEnumerator IE_Restart();

    bool Tick();

    bool FixedTick();

    bool LateTick();
}
