using System;
using System.Collections;
using UnityEngine;

public abstract class FileReader : IFileHandler
{
    public enum FileType
    {
        REMOTE = 0,
        PERSISTENT = 1,
        CSVFILE = 2
    }
#if UNITY_EDITOR
    static readonly string[] PROTOCOLS = { "file:///", "", "file:///" };
#else
#if UNITY_IPHONE
    static readonly string[] PROTOCOLS = { "file://", "","file://" };
#elif UNITY_ANDROID
    static readonly string[] PROTOCOLS = { "", "","file://" };
#endif
#endif

    public static FileReader Create(string fullpath, FileType type)
    {
        switch (type)
        {
            case FileType.REMOTE:
                {
                    return new RemoteFileReader(PROTOCOLS[(int)type] + fullpath);
                }
            case FileType.PERSISTENT:
                {
                    return new PersistentFileReader(PROTOCOLS[(int)type] + fullpath);
                }
            case FileType.CSVFILE:
                {
                    return new CSVFileReader(PROTOCOLS[(int)type] + fullpath);
                }
        }

        return null;
    }

    public string FullFileName { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool Success { get; private set; }
    public string TextData { get; private set; }

    public Action<FileReader> OnReadCompleted;

    private FileAsyncProcessor _loader;

    public FileReader(string fullpath)
    {
        FullFileName = fullpath;
    }

    public abstract IEnumerator DoAsync();

    public void ReadAsync()
    {
        IsCompleted = false;
#if NONE_DONTUSE
        TextData = FileUtils.ReadFile(FullFileName);
        OnCompleted(TextData);
        IsCompleted = true;
#else
        GameObject go = new GameObject(this.GetHashCode().ToString());
        _loader = go.AddComponent<FileAsyncProcessor>();
        _loader.LoadFile(this);
#endif
    }

    protected virtual void OnCompleted(string data)
    {
        IsCompleted = true;
        TextData = data;
#if !UNITY_EDITOR
        _loader.DestroySelf();
#endif
        Success = true;
        OnReadCompleted.Dispatch(this);
    }

    protected virtual void OnError(string error)
    {
        IsCompleted = true;
        Success = false;
        OnReadCompleted.Dispatch(this);
    }
}
