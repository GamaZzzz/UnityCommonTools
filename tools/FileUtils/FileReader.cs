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

    public static FileReader CreateReader(string sourcePath, string filePath, FileType type)
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
    /// <summary>
    /// 源路径
    /// </summary>
    public string SourcePath { get; private set; }
    /// <summary>
    /// 文件路径
    /// </summary>
    public string FilePath { get; private set; }
    /// <summary>
    /// 是否读取完毕（注意，无论读取成功或者失败，都会变成true）
    /// </summary>
    public bool IsCompleted { get; private set; }
    /// <summary>
    /// 是否读取成功
    /// </summary>
    public bool Success { get; private set; }
    /// <summary>
    /// text 数据
    /// </summary>
    public string TextData { get; private set; }
    /// <summary>
    /// 字节数据
    /// </summary>
    public byte[] ByteData { get; private set; }
    /// <summary>
    /// 读取完毕回调
    /// </summary>
    public Action<FileReader> OnReadCompleted;

    private FileAsyncProcessor _loader;

    public FileReader(string sourcePath, string filePath)
    {
        SourcePath = sourcePath;
        FilePath = filePath;
    }

    /// <summary>
    /// 执行异步读取（默认提供给FileAsyncProcessor调用）
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerator DoAsync();

    /// <summary>
    /// 开始异步读取
    /// </summary>
    public void ReadAsync()
    {
        DebugConsole.Info("Begin read file [" + SourcePath + FilePath + "] !");
        IsCompleted = false;
#if NONE
        TextData = FileUtils.ReadFile(SourcePath + FilePath);
        OnCompleted(TextData);
        IsCompleted = true;
#else
        GameObject go = new GameObject(FilePath.Replace("/",""));
        _loader = go.AddComponent<FileAsyncProcessor>();
        _loader.LoadFile(this);
#endif
    }

    protected virtual void OnCompleted(string data, byte[] bytes)
    {
        IsCompleted = true;
        TextData = data;
        ByteData = bytes;
        Success = true;
        DebugConsole.Info("Read [" + SourcePath + FilePath + "] success!");
        OnReadCompleted.Dispatch(this);
        if (_loader != null)
        {
            _loader.DestroySelf();
        }
    }

    protected virtual void OnError(string error)
    {
        IsCompleted = true;
        Success = false;
        DebugConsole.Error("Read [" + SourcePath + FilePath + "] faild!");
        OnReadCompleted.Dispatch(this);
        if (_loader != null)
        {
            _loader.DestroySelf();
        }
    }
}
