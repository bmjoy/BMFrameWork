using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T> : BaseSingleton<T> where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            return BaseInstance;
        }
    }

    protected virtual void Awake()
    {
        if (null == _instance)
        {
            _instance = Instance;
            DontDestroyOnLoad(this);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"This scene already have a instance to {typeof(T)}. Deleting duplicates.");
#endif
            Destroy(gameObject);
        }
    }
}