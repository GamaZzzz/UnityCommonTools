using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class FileAsyncProcessor : MonoBehaviour
{
    public void LoadFile(IFileHandler reader)
    {
        StartCoroutine(reader.DoAsync());
    }

    public void DestroySelf()
    {
        DestroyImmediate(gameObject);
    }
}

