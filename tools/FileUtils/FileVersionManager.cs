using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 文件版本管理器，
/// 使用异步加载方式，
/// 请时刻检查UpdateCompleted、LoadMd5Completed
/// 以便获知是否已经更新或者加载Md5版本文件
/// </summary>
public class FileVersionManager
{
    /// <summary>
    /// Md5版本文件是否加载完毕
    /// </summary>
    public bool LoadMd5Completed { get; private set; }
    /// <summary>
    /// 是否更新完毕
    /// </summary>
    public bool UpdateCompleted { get; private set; }
    /// <summary>
    /// 持久化Md5版本文件
    /// </summary>
    public string PersistentMd5File { get; private set; }
    /// <summary>
    /// 源Md5版本文件
    /// </summary>
    public string SourceMd5FileURL { get; private set; }
    /// <summary>
    /// 当前更新进度
    /// </summary>
    public float Progress { get; private set; }
    /// <summary>
    /// 变动过的文件数量
    /// </summary>
    public int ModifiedFiles { get; private set; }

    private string _localMd5 = "";
    private string _targetMd5 = "";
    private string _latestContent = "";

    private int _updatecount = 0;
    private int _processcount = 0;

    private object _progresslock = new object();

    private string _persistentPath = Application.persistentDataPath + "\\";

    private List<FileMd5Data> _latestFiles = new List<FileMd5Data>();

    /// <summary>
    /// 文件版本管理器
    /// </summary>
    /// <param name="md5file">本地持久化Md5版本文件</param>
    /// <param name="updatesrc">更新源Md5版本文件路径</param>
    public FileVersionManager(string localPersistentMd5File, string updateSourceMd5fileURL)
    {
        PersistentMd5File = localPersistentMd5File;
        SourceMd5FileURL = updateSourceMd5fileURL;
        UpdateCompleted = false;
    }

    /// <summary>
    /// 加载MD5版本文件
    /// </summary>
    public void LoadMd5VersionFile()
    {
        LoadMd5Completed = false;

        if (!File.Exists(PersistentMd5File))
        {
            FileReader rd = FileReader.Create(SourceMd5FileURL, FileReader.FileType.REMOTE);
            DebugConsole.Info("Load Remote: [" + rd.FullFileName + "] update faild!");
            rd.OnReadCompleted = (reader) => {
                GetModifiedFiles("", reader.TextData);
                LoadMd5Completed = true;
            };
            rd.ReadAsync();
        }
        else
        {
            FileReader rd = FileReader.Create(PersistentMd5File, FileReader.FileType.PERSISTENT);
            DebugConsole.Info("Load PERSISTENT: [" + rd.FullFileName + "] update faild!");
            rd.OnReadCompleted = (reader) =>
            {
                OnLoadCompleted(reader.TextData);
            };
            rd.ReadAsync();
        }
    }

    /// <summary>
    /// 开始异步更新持久化数据文件
    /// </summary>
    public void UpdatePersistentFilesAsync(string sourcePath)
    {
        if(ModifiedFiles > 0)
        {
            foreach (var md5filedata in _latestFiles)
            {
                FileReader reader = FileReader.Create(sourcePath + md5filedata.FullPath, FileReader.FileType.REMOTE);
                reader.OnReadCompleted = (rd) =>
                {
                    FileWriter writer = new PersistentFileWriter(rd.TextData, _persistentPath + md5filedata.FullPath);
                    writer.OnWriteCompleted = OnWirteComplete;
                    writer.WriteAsync();
                };
                reader.ReadAsync();
            }
        }
        else
        {
            UpdateCompleted = true;
        }
    }

    void OnWirteComplete(FileWriter writer)
    {
        if (writer.IsCompleted)
        {
            IncProgress(true);
#if UNITY_EDITOR
            Debug.Log("File [" + writer.FullFileName + "] update success!");
#else
            DebugConsole.Info("File [" + writer.FullFileName + "] update success!");
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("File [" + writer.FullFileName + "] update faild!");
#else
            DebugConsole.Info("File [" + writer.FullFileName + "] update faild!");
#endif
        }
    }

    void IncProgress(bool success)
    {
        lock (_progresslock)
        {
            if (success)
            {
                _updatecount++;
                Progress = (float)_updatecount / ModifiedFiles;
            }

            _processcount++;

            UpdateCompleted = _processcount == ModifiedFiles;

            if(_updatecount == ModifiedFiles)
            {
                FileUtils.WritePersistentFile(_latestContent, PersistentMd5File);
            }
        }
    }

    void OnLoadCompleted(string data)
    {
        _localMd5 = FileUtils.GetMd5(data);
        FileReader rd = FileReader.Create(SourceMd5FileURL, FileReader.FileType.REMOTE);
        DebugConsole.Info("Load SourceMd5FileURL: [" + rd.FullFileName + "] update faild!");
        rd.OnReadCompleted = (reader) => {
            _targetMd5 = FileUtils.GetMd5(reader.TextData);
            if(_localMd5 == _targetMd5)
            {
#if UNITY_EDITOR
                Debug.Log("Md5File:" + PersistentMd5File + " Don't need to update!");
#else
                DebugConsole.Info("Md5File:" + PersistentMd5File + "Don't need to update!");
#endif
            }
            GetModifiedFiles(data, reader.TextData);
            LoadMd5Completed = true;
        };
        rd.ReadAsync();
    }

    void GetModifiedFiles(string md5filecontent, string newVerMd5filecontent)
    {
        _latestContent = newVerMd5filecontent;//缓存最新版Md5文件
        ModifiedFiles = 0;
        _latestFiles.Clear();
        FileMd5DataDictionary newMd5DataDictionary = new FileMd5DataDictionary();
        newMd5DataDictionary.Parse(newVerMd5filecontent);

        if (!string.IsNullOrEmpty(md5filecontent))
        {
            FileMd5DataDictionary curMd5DataDictionary = new FileMd5DataDictionary();
            curMd5DataDictionary.Parse(md5filecontent);
            foreach(var newVer in newMd5DataDictionary.GetAllValues())
            {
                var oldVer = curMd5DataDictionary.Get(newVer.FileName);
                if(oldVer == null || oldVer.Md5Code != newVer.Md5Code)
                {
                    _latestFiles.Add(newVer);
                    ModifiedFiles++;
                }
            }
        }
        else
        {
            ModifiedFiles = newMd5DataDictionary.Count;
            _latestFiles.AddRange(newMd5DataDictionary.GetAllValues());
        }
    }
}

