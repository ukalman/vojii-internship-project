using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleEventManager
{
    private static ModuleEventManager instance;

    public static ModuleEventManager Instance => instance ?? (instance = new ModuleEventManager());

    private Dictionary<Type, Action<CustomEvent>> eventListeners = new Dictionary<Type, Action<CustomEvent>>();

    public void Subscribe<T>(Action<T> listener) where T : CustomEvent
    {
        if (!eventListeners.ContainsKey(typeof(T)))
        {
            eventListeners[typeof(T)] = delegate { };
        }

        eventListeners[typeof(T)] += (e) => listener((T)e);
    }

    public void Unsubscribe<T>(Action<T> listener) where T : CustomEvent
    {
        if (eventListeners.ContainsKey(typeof(T)))
        {
            eventListeners[typeof(T)] -= (e) => listener((T)e);
        }
    }

    public void Publish<T>(T eventToPublish) where T : CustomEvent
    {
        if (eventListeners.ContainsKey(typeof(T)))
        {
            eventListeners[typeof(T)].Invoke(eventToPublish);
        }
    }
}

