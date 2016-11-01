using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class FileReader
{
    public bool LoadCompleted { get; private set; }
    public FileStream FileStr;
    public StreamReader Sr;
    public byte[] Buffer;

    public void LoadFile(string path)
    {
        LoadCompleted = false;
#if UNITY_EDITOR || UNITY_IPHONE
        try
        {
            LoadCompleted = false;
            FileStr = File.OpenRead(path);       
            Buffer = new byte[FileStr.Length];
            FileStr.BeginRead(Buffer, 0, (int)FileStr.Length, OnReadCallback, this);
        }
        catch
        {
        #if UNITY_EDITOR
             EditorApplication.isPlaying = false;
        #else
             Application.Quit();
        #endif
        }
#else
        CoroutineExecutor.ExecuteCoroutine(DoLoadFile(path));
#endif
    }

    private IEnumerator DoLoadFile(string path)
    {
        using (WWW uwr = new WWW(path))
        {
            
            yield return uwr;
            if (uwr.isDone)
            {
                Buffer = Encoding.UTF8.GetBytes(uwr.text);
            }
            LoadCompleted = true;
        }
    }

    private void OnReadCallback(IAsyncResult iar)
    {
        FileReader re = iar.AsyncState as FileReader;
        try
        {
            re.FileStr.EndRead(iar);
            if (iar.IsCompleted)
            {
                Debug.Log("Read Successfully!");
                LoadCompleted = true;
            }
        }
        catch
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        finally
        {
            FileStr.Close();
            FileStr.Dispose();
        }
    }
}

