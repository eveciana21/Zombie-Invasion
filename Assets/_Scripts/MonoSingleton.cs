using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T _instance;
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
        Init();
        //_instance = (T)this;
    }

    public virtual void Init()
    {
        _instance = (T)this;
        // has nothing in this method
        //optional to override and convert overriden manager into singleton
    }
}


