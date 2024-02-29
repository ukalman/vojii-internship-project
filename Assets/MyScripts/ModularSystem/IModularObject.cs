using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModularObject 
{

    List<IModule> Modules { get; set; }
    Dictionary<Type, IModule> ModuleTypesDictionary { get; set; }

    T GetModule<T>() where T : class, IModule;

    IEnumerator IE_ActivateModules();

    IEnumerator IE_DeactivateModules();

    IEnumerator IE_ResetAllModules();
}
