using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;
    public static SceneLoader Instance
    {
        get
        {
            return _instance;
        }
    }

    private LoadingScreen loadingScreen;

    void Awake()
    {
        loadingScreen = GetComponentInChildren<LoadingScreen>();
        _instance = this;
        DontDestroyOnLoad(this);
        //loadingScreen.Hide();
    }

    public event Action OnLoadBegin;
    public event Action OnLoadEnd;
    public event Action<float> OnProgress;

    public void LoadNextLevel(bool additive = false)
    {
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        this.LoadSceneAsync(Application.loadedLevel + 1, additive);
#else
        this.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1, additive);
#endif
    }

    public void LoadLastLevel(bool additive = false)
    {
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        if(Application.loadedLevel > 0){
            this.LoadSceneAsync(Application.loadedLevel - 1, additive);
        } 
#else
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex > 0)
        {
            this.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1, additive);
        }
#endif
    }

    public void LoadSceneAsync(string scenename, bool additive = false)
    {
        StartCoroutine(WaitForLoadLevel(scenename, additive));
    }

    public void LoadSceneAsync(string scene1, string scene2)
    {
        StartCoroutine(LoadSceneQueueAdditive(scene1, scene2));
    }

    public void LoadSceneAsync(int level, bool additive = false)
    {
        StartCoroutine(WaitForLoadLevel(level, additive));
    }

    IEnumerator LoadSceneQueueAdditive(string scene1, string scene2)
    {
        yield return WaitForLoadLevel(scene1);

        yield return WaitForLoadLevel(scene2, true);
    }

    IEnumerator WaitForLoadLevel(int level , bool additive = false)
    {
        loadingScreen.Show();
        OnLoadBegin.Dispatch();
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        AsyncOperation oper = additive ?  Application.LoadLevelAdditiveAsync(level) : Application.LoadLevelAsync(level);
#else
        AsyncOperation oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
#endif

        OnProgress.Dispatch(oper.progress);

        while (!oper.isDone)
        {
            yield return new WaitForFixedUpdate();
            OnProgress.Dispatch(oper.progress);
            loadingScreen.OnProgress(oper.progress);
        }

        OnLoadEnd.Dispatch();
        loadingScreen.Hide();
    }

    IEnumerator WaitForLoadLevel(string level, bool additive = false)
    {
        loadingScreen.Show();
        OnLoadBegin.Dispatch();

#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
        AsyncOperation oper = additive ?  Application.LoadLevelAdditiveAsync(level) : Application.LoadLevelAsync(level);
#else
        AsyncOperation oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(level, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
#endif
        OnProgress.Dispatch(oper.progress);

        while (!oper.isDone)
        {
            yield return new WaitForFixedUpdate();
            OnProgress.Dispatch(oper.progress);
            loadingScreen.OnProgress(oper.progress);
        }

        yield return new WaitForSeconds(1f);

        OnLoadEnd.Dispatch();
        loadingScreen.Hide();
    }

}

