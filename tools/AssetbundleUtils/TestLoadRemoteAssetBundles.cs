using UnityEngine;
using System.Collections;
using System;

public class TestLoadRemoteAssetBundles : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadRemoteAssetBundle());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator LoadRemoteAssetBundle()
    {
        while (!Caching.ready)
            yield return null;

        using (var www = WWW.LoadFromCacheOrDownload("http://localhost/Assetbundles/scriptdata.unity3d",1))
        {
            yield return www;

            if (www.isDone)
            {
                if (www.error != null)
                {
                    throw new Exception("WWW download had an error:" + www.error);
                }

                AssetBundle bundle = www.assetBundle;

                GameObject go = Instantiate(bundle.mainAsset) as GameObject;
            }
        }
    }
}
