using Sirenix.OdinInspector;
using UnityEngine;

public class Singleton<T>: SerializedMonoBehaviour where T:Component{
    protected static bool Destory = false;
    private static object _lock = new object ();
    private static T instance;
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (Destory && !GlobalGameData.ResetSingleton)
                    return null;

                if (instance == null)
                {
                    instance = FindObjectOfType(typeof (T)) as T;
                    if (instance == null)
                    {
                        GameObject  go = new GameObject();
                        go.name = typeof (T).Name;
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    protected virtual void OnDestroy()
    {
        Destory = true;
    }
}