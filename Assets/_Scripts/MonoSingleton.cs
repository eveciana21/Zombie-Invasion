using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError(typeof(T).ToString() + " is NULL");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = (T)this;
    }

    public virtual void Init()
    {
        //optional to override and convert overriden manager into singleton
    }
}
