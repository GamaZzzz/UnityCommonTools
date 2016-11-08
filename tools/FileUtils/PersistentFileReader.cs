using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 持久化文件读取，使用FileStream方式
/// </summary>
public class PersistentFileReader : FileReader
{
    public FileStream ReadStream { get; private set; }
    public byte[] ByteDatas { get; private set; }

    public PersistentFileReader(string sourcePath, string filePath) : base(sourcePath, filePath)
    {

    }

    public override IEnumerator DoAsync()
    {
        ReadStream = File.OpenRead(this.SourcePath + this.FilePath);
        ByteDatas = new byte[ReadStream.Length];
        ReadStream.BeginRead(ByteDatas, 0, (int)ReadStream.Length, new AsyncCallback(OnReadStreamCallback), this);
        yield break;     
    }

    void OnReadStreamCallback(IAsyncResult iar)
    {
        PersistentFileReader pfr = iar.AsyncState as PersistentFileReader;
        try
        {
            int ret = pfr.ReadStream.EndRead(iar);
            if(ret > 0)
            {
                if (iar.IsCompleted)
                {
                    this.OnCompleted(Encoding.UTF8.GetString(pfr.ByteDatas), pfr.ByteDatas);
                }
            }
        }
        catch(Exception e)
        {
            this.OnError(e.Message);
        }
        finally
        {
            pfr.ReadStream.Close();
            pfr.ReadStream.Dispose();
            pfr.ReadStream = null;
        }
    }
}

