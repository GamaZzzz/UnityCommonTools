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
    static readonly string[] PROTOCOLS = { "", "", "file://" };
#endif
#endif

    public static FileReader Create(string sourcePath, string filePath, FileType type)
    {
        switch (type)
        {
            case FileType.REMOTE:
                {
                    return new RemoteFileReader(PROTOCOLS[(int)type] + sourcePath, filePath);
                }
            case FileType.PERSISTENT:
                {
                    return new PersistentFileReader(PROTOCOLS[(int)type] + sourcePath, filePath);
                }
            case FileType.CSVFILE:
                {
                    return new CSVFileReader(PROTOCOLS[(int)type] + sourcePath, filePath);
                }
        }

        return null;
    }
    public string SourcePath { get; private set; }
    public string FilePath { get; private set; }
    public bool IsCompleted { get; private set; }
    public bool Success { get; private set; }
    public string TextData { get; private set; }
    public byte[] ByteData { get; private set; }

    public Action<FileReader> OnReadCompleted;

    private FileAsyncProcessor _loader;

    public FileReader(string sourcePath, string filePath)
    {
        SourcePath = sourcePath;
        FilePath = filePath;
    }

    public abstract IEnumerator DoAsync();

    public void ReadAsync()
    {
        DebugConsole.Info("Begin read file [" + SourcePath + FilePath + "] !");
        IsCompleted = false;
#if NONE
        TextData = FileUtils.ReadFile(SourcePath + FilePath);
        OnCompleted(TextData);
        IsCompleted = true;
#else
        GameObject go = new GameObject(this.GetHashCode().ToString());
        _loader = go.AddComponent<FileAsyncProcessor>();
        _loader.LoadFile(this);
#endif
    }

    protected virtual void OnCompleted(string data, byte[] bytes)
    {
        IsCompleted = true;
        TextData = data;
        ByteData = bytes;
#if !UNITY_EDITOR
        _loader.DestroySelf();
#endif
        Success = true;
        DebugConsole.Info("Read [" + SourcePath + FilePath + "] success!");
        OnReadCompleted.Dispatch(this);
    }

    protected virtual void OnError(string error)
    {
        IsCompleted = true;
        Success = false;
        DebugConsole.Error("Read [" + SourcePath + FilePath + "] faild!");
        OnReadCompleted.Dispatch(this);
    }
}
