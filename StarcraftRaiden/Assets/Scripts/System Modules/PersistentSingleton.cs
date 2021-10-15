using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if(Instance == null)
        {
            Instance = this as T;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        //加載場景時不刪除此物件
        DontDestroyOnLoad(gameObject);
    }
}
