using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CoroutineExecutor : MonoBehaviour
{
    private static CoroutineExecutor _instance;
    public static CoroutineExecutor Instance
    {
        get
        {
            if(null == _instance)
            {
                GameObject go = new GameObject("CoroutineExecutor");
                _instance = go.AddComponent<CoroutineExecutor>();
            }
            return _instance;
        }
    }

    public static void ExecuteCoroutine(IEnumerator coroutine)
    {
        Instance.DoStartCoroutine(coroutine);
    }

    private void DoStartCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }

    void Start()
    {
        DontDestroyOnLoad(this);
    }
}

