using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PersistentFileWriter : FileWriter
{
    public FileStream WriteStream { get; private set; }
    public byte[] WriteBytes { get; private set; }

    public PersistentFileWriter(string content, string fullpath) : base(content, fullpath)
    {
        WriteBytes = Encoding.UTF8.GetBytes(content);
    }

    public override IEnumerator DoAsync()
    {
        if (!File.Exists(FullFileName))
        {
            DirectoryInfo di = new DirectoryInfo(FullFileName);
            if (!di.Parent.Exists)
            {
                di.Parent.Create();
#if UNITY_EDITOR
                Debug.Log("Create Dir:" + di.Parent.FullName);
#else
                DebugConsole.Info("Create Dir:"+ di.Parent.FullName);
#endif
            }

            FileStream file = File.Create(FullFileName);
            file.Close();
            file.Dispose();
            file = null;
        }

        WriteStream = File.OpenWrite(FullFileName);
        WriteStream.BeginWrite(WriteBytes, 0, WriteBytes.Length, new AsyncCallback(On_Write_Callback), this);

        yield break;
    }

    void On_Write_Callback(IAsyncResult iar)
    {
        PersistentFileWriter pfw = iar.AsyncState as PersistentFileWriter;
        try
        {
            pfw.WriteStream.EndWrite(iar);
            if (iar.IsCompleted)
            {
                this.OnCompleted();
            }
        }
        catch(Exception e)
        {
            this.OnError(e.Message);
        }
        finally
        {
            pfw.WriteStream.Close();
            pfw.WriteStream.Dispose();
            pfw.WriteStream = null;
        }
    }
}

