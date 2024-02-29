using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : MonoBehaviour, IModule
{

    [field: SerializeField]
    public int Priority { get; set; }

    [field: SerializeField]
    protected string moduleName;

    [field: SerializeField]
    public ModuleState State { get; set; }

    [field: SerializeField]
    public IModularObject Parent { get; set; }


    public virtual IEnumerator IE_Activate()
    {
        gameObject.SetActive(true);
        yield return null;
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator IE_SetState(ModuleState newState)
    {

        ModuleEventManager.Instance.Publish(new BeforeModuleStateChangedEvent(this, newState, "Before changing state"));

        // Change the module state
        var previousState = State;
        State = newState;

        switch (State)
        {
            // deinitialize ol
            case ModuleState.Uninitialized:
                yield return StartCoroutine(IE_Deinitialize());
                break;
            case ModuleState.Initialized:
                yield return StartCoroutine(IE_Initialize());
                break;
            case ModuleState.PostInitialized:
                yield return StartCoroutine(IE_PostInitialize());
                break;
            case ModuleState.Activated:
                yield return StartCoroutine(IE_Activate());
                break;
            case ModuleState.Deactivated:
                Deactivate();
                break;
            case ModuleState.Disabled:
                yield return StartCoroutine(IE_Disable());
                break;
            case ModuleState.Paused:
                yield return StartCoroutine(IE_Pause());
                break;
            case ModuleState.Resumed:
                yield return StartCoroutine(IE_Resume());
                break;
            case ModuleState.Restarted:
                yield return StartCoroutine(IE_Restart());
                break;
            default:
                break;
        }

        // Publish the after state change event
        ModuleEventManager.Instance.Publish(new AfterModuleStateChangedEvent(this, newState, "After changing state"));
    }

    public IModularObject GetParent()
    {
        return this.Parent;
    }

    public IEnumerator SetParent(IModularObject Parent)
    {
        this.Parent = Parent;
        yield return null;
    }

    public string GetName()
    {
        return this.moduleName;
    }

    public virtual IEnumerator IE_Initialize()
    {
        yield return null;
    }

    public virtual IEnumerator IE_PostInitialize()
    {
        yield return null;
    }

    public virtual IEnumerator IE_Deinitialize()
    {
        yield return null;
    }

    public virtual IEnumerator IE_Disable()
    {
        yield return null;
    }

    public virtual IEnumerator IE_Pause()
    {
        yield return null;
    }

    public virtual IEnumerator IE_Resume()
    {
        yield return null;
    }

    public virtual IEnumerator IE_Restart()
    {
        yield return null;
    }

    public virtual bool Tick() {

        if (State != ModuleState.Activated)
        {
            return false;
        }

        return true;
    }


    public virtual bool FixedTick()
    {
        if (State != ModuleState.Activated)
        {
            return false;
        }

        return true;
    }

    public virtual bool LateTick()
    {
        if (State != ModuleState.Activated)
        {
            return false;
        }

        return true;
    }






}
