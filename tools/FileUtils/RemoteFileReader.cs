using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 远程文件读取，WWW方式file:\\协议读取
/// </summary>
public class RemoteFileReader : FileReader
{
    public RemoteFileReader(string sourcePath, string filePath) : base(sourcePath, filePath)
    { 
        
    }

    public override IEnumerator DoAsync()
    {
        using (WWW www = new WWW(SourcePath + FilePath))
        {
            yield return www;

            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                DebugConsole.Info("Bytes:" + www.bytesDownloaded);
                OnCompleted(www.text, www.bytes);
            }
            else
            {
                OnError(www.error);
            }
        }
    }
}

