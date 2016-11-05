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
            if(_instance == null)
            {
                GameObject go = new GameObject("CoroutineExecutor");
                _instance = go.AddComponent<CoroutineExecutor>();
                GameObject.DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void ExecuteCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}

