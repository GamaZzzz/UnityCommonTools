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
    public bool LoadMd5Success { get; private set; }
    /// <summary>
    /// 是否更新完毕
    /// </summary>
    public bool UpdateCompleted { get; private set; }
    public bool UpdateSuccess { get; private set; }
    /// <summary>
    /// 持久化目录
    /// </summary>
    public string PersistentBase { get; private set; }
    /// <summary>
    /// 持久化Md5版本文件
    /// </summary>
    public string PersistentMd5File { get; private set; }
    /// <summary>
    /// 更新源文件基础URL
    /// </summary>
    public string SourceBase { get; private set; }
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
    private int _deletedcount = 0;

    private object _progresslock = new object();

    private List<FileMd5Data> _latestFiles = new List<FileMd5Data>();
    private List<FileMd5Data> _deletedFiles = new List<FileMd5Data>();

    /// <summary>
    /// 文件版本管理器
    /// </summary>
    /// <param name="md5file">本地持久化Md5版本文件</param>
    /// <param name="updatesrc">更新源Md5版本文件路径</param>
    public FileVersionManager(string persistentBase, 
        string localPersistentMd5File, 
        string sourceBase,
        string updateSourceMd5fileURL)
    {
        PersistentBase = persistentBase;
        PersistentMd5File = localPersistentMd5File;
        SourceMd5FileURL = updateSourceMd5fileURL;
        SourceBase = sourceBase;
        UpdateCompleted = false;
    }

    /// <summary>
    /// 加载MD5版本文件
    /// </summary>
    public void LoadMd5VersionFile()
    {
        Init();

        if (!File.Exists(PersistentBase + PersistentMd5File))
        {
            FileReader rd = FileReader.CreateReader(SourceBase, SourceMd5FileURL, FileReader.FileType.REMOTE);
            
            rd.OnReadCompleted = (reader) => {
                if (reader.Success)
                {
                   //DebugConsole.Info("Load Remote: [" + rd.FullFileName + "] success!");
                    GetModifiedFiles("", reader.TextData);
                }
                else
                {
                    DebugConsole.Info("Load PERSISTENT: [" + rd.SourcePath + rd.FilePath + "] update faild!");
                }
                LoadMd5Success = reader.Success;
                LoadMd5Completed = true;
            };
            rd.ReadAsync();
        }
        else
        {
            FileReader rd = FileReader.CreateReader(PersistentBase, PersistentMd5File, FileReader.FileType.PERSISTENT);
           
            rd.OnReadCompleted = (reader) =>
            {
                if (reader.Success)
                {
                    OnLoadCompleted(reader.TextData);
                }
                else
                {
                    DebugConsole.Info("Load PERSISTENT: [" + rd.SourcePath + rd.FilePath + "] update faild!");
                    LoadMd5Success = reader.Success;
                    LoadMd5Completed = true;
                }
            };
            rd.ReadAsync();
        }
    }

    /// <summary>
    /// 开始异步更新持久化数据文件
    /// </summary>
    public void UpdatePersistentFilesAsync(string sourcePath)
    {
        if (_deletedcount > 0)
        {
            //先删除已经丢弃的文件
            foreach(var item in _deletedFiles)
            {
                if(File.Exists(PersistentBase + item.FullPath))
                {
                    File.Delete(PersistentBase + item.FullPath);
                }
            }
        }

        if(ModifiedFiles > 0)
        {
            foreach (var md5filedata in _latestFiles)
            {
                DebugConsole.Info("Begin update [" + md5filedata.FullPath + "] !");
                FileReader reader = FileReader.CreateReader(sourcePath , md5filedata.FullPath, FileReader.FileType.REMOTE);
                reader.OnReadCompleted = (rd) =>
                {
                    FileWriter writer = new PersistentFileWriter(rd.ByteData, PersistentBase + rd.FilePath);
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
            Debug.LogError("File [" + writer.FullFileName + "] update faild!");
#else
            DebugConsole.Error("File [" + writer.FullFileName + "] update faild!");
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
                UpdateSuccess = true;
                FileUtils.WritePersistentFile(_latestContent, PersistentBase + PersistentMd5File);
            }
            else
            {
                UpdateSuccess = false;
            }
        }
    }

    void OnLoadCompleted(string data)
    {
        _localMd5 = FileUtils.GetMd5(data);
        FileReader rd = FileReader.CreateReader(SourceBase, SourceMd5FileURL, FileReader.FileType.REMOTE);
        //DebugConsole.Info("Load SourceMd5FileURL: [" + rd.FullFileName + "] update success!");
        rd.OnReadCompleted = (reader) => {
            if (reader.Success)
            {
                _targetMd5 = FileUtils.GetMd5(reader.TextData);
                if (_localMd5 == _targetMd5)
                {
#if UNITY_EDITOR
                    Debug.Log("Md5File:" + PersistentBase + PersistentMd5File + " Don't need to update!");
#else
                DebugConsole.Info("Md5File:" + PersistentMd5File + "Don't need to update!");
#endif
                }
                GetModifiedFiles(data, reader.TextData);
            }
            LoadMd5Success = reader.Success;
            LoadMd5Completed = true;
        };
        rd.ReadAsync();
    }

    void Init()
    {
        ModifiedFiles = 0;
        _updatecount = 0;
        _processcount = 0;
        _deletedcount = 0;
        _latestFiles.Clear();
        _deletedFiles.Clear();
        LoadMd5Completed = false;
        LoadMd5Success = false;
        UpdateCompleted = false;
        UpdateSuccess = false;
        Progress = 0f;
    }

    void GetModifiedFiles(string md5filecontent, string newVerMd5filecontent)
    {
        _latestContent = newVerMd5filecontent;//缓存最新版Md5文件

        FileMd5DataDictionary newMd5DataDictionary = new FileMd5DataDictionary();
        newMd5DataDictionary.Parse(newVerMd5filecontent);

        if (!string.IsNullOrEmpty(md5filecontent))
        {
            FileMd5DataDictionary curMd5DataDictionary = new FileMd5DataDictionary();
            curMd5DataDictionary.Parse(md5filecontent);
            List<FileMd5Data> tempNewVers = newMd5DataDictionary.GetAllValues();
            foreach (var newVer in tempNewVers)
            {
                var oldVer = curMd5DataDictionary.Get(newVer.FileName);
                if(oldVer == null || oldVer.Md5Code != newVer.Md5Code)
                {
                    _latestFiles.Add(newVer);
                    ModifiedFiles++;
                }
            }
            List<FileMd5Data> temp = curMd5DataDictionary.GetAllValues();
            foreach (var item in temp)
            {
                if (!newMd5DataDictionary.ContainsKey(item.FileName))
                {
                    _deletedFiles.Add(item);
                    _deletedcount++;
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

