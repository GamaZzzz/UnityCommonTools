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

    public PersistentFileWriter(string content, string fullpath) : base(Encoding.UTF8.GetBytes(content), fullpath)
    {

    }

    public PersistentFileWriter(byte[] content, string fullpath) : base(content, fullpath)
    {

    }

    public override void  DoAsync()
    {
        if (!File.Exists(FullFileName))
        {
            DirectoryInfo di = new DirectoryInfo(FullFileName);
            if (!di.Parent.Exists)
            {
                CreateDir(di.Parent);
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
        WriteStream.SetLength(0);
        WriteStream.BeginWrite(Content, 0, Content.Length, new AsyncCallback(On_Write_Callback), this);
    }

    void CreateDir(DirectoryInfo dirInfo)
    {
        if (!dirInfo.Parent.Exists)
        {
            CreateDir(dirInfo.Parent);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("Create Dir:" + dirInfo.FullName);
#else
                        DebugConsole.Info("Create Dir:" + dirInfo.FullName);
#endif
            //dirInfo.Create();
            Directory.CreateDirectory(dirInfo.FullName);
        }
    }

    void On_Write_Callback(IAsyncResult iar)
    {
        PersistentFileWriter pfw = iar.AsyncState as PersistentFileWriter;
        try
        {
            pfw.WriteStream.EndWrite(iar);
            if (iar.IsCompleted)
            {
                pfw.WriteStream.Flush();
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

