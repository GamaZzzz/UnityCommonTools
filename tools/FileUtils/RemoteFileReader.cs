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
    public RemoteFileReader(string fullpath) : base(fullpath)
    {
        
    }

    public override IEnumerator DoAsync()
    {
        using (WWW www = new WWW(FullFileName))
        {
            yield return www;

            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                OnCompleted(www.text);
            }
            else
            {
                DebugConsole.Error(www.error);
                OnError(www.error);
            }
        }
    }
}

