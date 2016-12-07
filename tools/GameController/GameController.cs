using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance
    {
        get { return instance; }
    }

    public static FPSCounter FpsCounter { get; private set; }

    private Dictionary<string, MonoBehaviour> modules = new Dictionary<string, MonoBehaviour>();

    static GameController()
    {
        GameObject go = new GameObject("GameController");
        instance = go.AddComponent<GameController>();
        FpsCounter = go.AddComponent<FPSCounter>();
        FpsCounter.Show = true;
        DontDestroyOnLoad(go);
    }

    /// <summary>
    /// 注册持久化模块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RegisterPersistentModule<T>() where T : MonoBehaviour
    {
        MonoBehaviour mono = gameObject.AddComponent<T>();
        modules.Add(typeof(T).Name, mono);
    }

    public T GetModule<T>() where T : MonoBehaviour
    {
        string key = typeof(T).Name;
        return GetModule<T>(key);
    }

    public T GetModule<T>(string name) where T : MonoBehaviour
    {
        if (modules.ContainsKey(name))
        {
            return modules[name] as T;
        }
        return null;
    }
}
