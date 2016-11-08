using System;
using System.Collections;
using UnityEngine;

public abstract class FileWriter
{
    public bool IsCompleted { get; private set; }
    public byte[] Content { get; private set; }
    public string FullFileName { get; private set; }

    public Action<FileWriter> OnWriteCompleted;

    private FileAsyncProcessor _loader;

    public FileWriter(byte[] content, string fullpath)
    {
        Content = content;
        FullFileName = fullpath;
        IsCompleted = false;
    }

    public void WriteAsync()
    {
        DebugConsole.Info("Begin Wirete [" + FullFileName + "] !");
        IsCompleted = false;
#if NONE
        FileUtils.WritePersistentFile(Content, FullFileName);
        OnCompleted();
        IsCompleted = true;
#else
        DoAsync();
#endif
    }

    public abstract void DoAsync();

    protected virtual void OnCompleted()
    {
        IsCompleted = true;
        OnWriteCompleted.Dispatch(this);
    }

    protected virtual void OnError(string error)
    {
        IsCompleted = false;
        OnWriteCompleted.Dispatch(this);
    }
}

