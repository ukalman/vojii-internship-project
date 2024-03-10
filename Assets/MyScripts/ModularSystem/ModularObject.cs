using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// ReSharper disable All



public abstract class ModularObject : MonoBehaviour, IModularObject {

    public Dictionary<Type, IModule> ModuleTypesDictionary { get; set; }


    [SerializeField]
    public List<MonoBehaviour> _modules = new();

    public List<IModule> Modules
    {
        get => _modules.Cast<IModule>().ToList();
        set => _modules = value.Cast<MonoBehaviour>().ToList();
    }


    private void Awake()
    {
        StartCoroutine(SetupModules());
    }

    public IEnumerator SetupModules()
    {
        ModuleTypesDictionary = new Dictionary<Type, IModule>();
        yield return StartCoroutine(RegisterAllModules());
        
    }

    private void Start()
    {
        //ActivateModules();
    }

    public void LogContentsOfList(List<IModule> list) {
        foreach (var item in list)
        {
            Debug.Log(item.GetName());
        }
    }

    public IEnumerator RegisterAllModules()
    {
        Modules.RemoveAll(module => module == null);

        Modules = Modules.OrderBy(m => m.Priority).ToList();
        _modules = Modules.Select(m => m as MonoBehaviour).Where(m => m != null).ToList();
        //Debug.Log("Modules are being registered");
        //Debug.Log("Module names:");
        //LogContentsOfList(Modules);
       

        if (Modules.Count > 0)
        {
            foreach (var module in Modules)
            {
                if(module.State != ModuleState.Activated) yield return StartCoroutine(AddModule(module));

            }
        }
      

    }

    public void RegisterAllModulesFromEditor()
    {
        StartCoroutine(RegisterAllModules());
    }

    private IEnumerator AddModule(IModule module)
    {
     

        ModuleTypesDictionary[module.GetType()] = module;
        if (Application.isPlaying)
        {
            yield return StartCoroutine(module.SetParent(this)); 
            yield return StartCoroutine(module.IE_Initialize());
            yield return StartCoroutine(module.IE_PostInitialize());
            yield return StartCoroutine(module.IE_SetState(ModuleState.Activated));
        }
    }
    
    // public void ActivateModules()
    // {
    //     foreach (var module in Modules)
    //     {
    //         if(module != null) module.Activate();
    //     }
    // }

    public IEnumerator IE_ActivateModules()
    {
        foreach (var module in Modules)
        {
            if (module != null) yield return module.IE_Activate();
        }
    }
    
    public IEnumerator IE_DeactivateModules()
    {
        foreach (var module in Modules)
        {
            if (module != null)  module.Deactivate(); //yield return module.IE_Deactivate();
        }

        yield return null;
    }

    private void Update()
    {
        foreach (var module in Modules)
        {
            if (module != null)  module.Tick();
        }
    }

    private void FixedUpdate()
    {
        foreach(var module in Modules)
        {
            if (module != null)  module.FixedTick();
        }
    }

    private void LateUpdate()
    {
        foreach (var module in Modules)
        {
            if (module != null)  module.LateTick();
        }
    }

    public T GetModule<T>() where T : class, IModule
    {
        if (ModuleTypesDictionary.TryGetValue(typeof(T), out IModule module))
        {
            return module as T;
        }

        return null;
    }


    public virtual IEnumerator IE_ResetAllModules()
    {
        for (int i = Modules.Count - 1; i >= 0; i--)
        {
            if (Modules[i] != null)
            {
                Debug.Log("Resetting module: " + Modules[i].GetName());
                yield return Modules[i].IE_Restart();
            }
        }
        
        /*
        foreach (var module in Modules)
        {
            if (module != null)
            {   
                
            } 
        }
        */
    }

   
}