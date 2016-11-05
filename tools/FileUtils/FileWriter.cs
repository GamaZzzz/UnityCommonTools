using System;
using System.Collections;
using UnityEngine;

public abstract class FileWriter : IFileHandler
{
    public bool IsCompleted { get; private set; }
    public string Content { get; private set; }
    public string FullFileName { get; private set; }

    public Action<FileWriter> OnWriteCompleted;

    private FileAsyncProcessor _loader;

    public FileWriter(string content, string fullpath)
    {
        Content = content;
        FullFileName = fullpath;
        IsCompleted = false;
    }

    public void WriteAsync()
    {
        IsCompleted = false;
#if NONE_DONTUSE
        FileUtils.WritePersistentFile(Content, FullFileName);
        OnCompleted();
        IsCompleted = true;
#else
        GameObject go = new GameObject(this.GetHashCode().ToString());
        _loader = go.AddComponent<FileAsyncProcessor>();
        _loader.LoadFile(this);
#endif
    }

    public abstract IEnumerator DoAsync();

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

