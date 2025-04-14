using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Singleton<T>: SerializedMonoBehaviour where T:Component{
    private static readonly object _lock = new();
    private static T instance;
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType(typeof(T)) as T;
                    if (instance == null)
                    {
                        GameObject go = new()
                        {
                            name = typeof(T).Name
                        };
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
    }
}