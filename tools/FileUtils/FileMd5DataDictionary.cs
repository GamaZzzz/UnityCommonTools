using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class FileMd5DataDictionary : BaseDictionary<string, FileMd5Data>
{
    public int Count { get; private set; }
    protected override string Path
    {
        get
        {
            return "FilesMd5.csv";
        }
    }

    protected override string BasePath
    {
        get
        {
            return "";
        }
    }

    protected override void ParseLine(List<string[]> datas)
    {
        _dictrionary = new Dictionary<string, FileMd5Data>();
        int count = datas.Count;
        Count = 0;
        for (int i = 1; i < count; ++i)
        {
            FileMd5Data data = new FileMd5Data();
            data.Parse(datas[i]);
            _dictrionary.Add(data.FileName, data);
            Count += 1;
        }
    }

    public void Parse(string content)
    {
        using (StringReader reader = new StringReader(content))
        {
            List<string[]> result = new List<string[]>();
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                result.Add(line.Split(','));
                line = reader.ReadLine();
            }
            ParseLine(result);
        }
    }

    public void AddData(FileMd5Data data)
    {
        if (_dictrionary == null)
        {
            _dictrionary = new Dictionary<string, FileMd5Data>();
        }

        if (!_dictrionary.ContainsKey(data.FileName))
        {
            _dictrionary.Add(data.FileName, data);
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("File:[" + data.FileName + "] already exist!");
#else
            DebugConsole.Error("File:[" + data.FileName + "] already exist!");
#endif
        }
    }

    public void UpdateMd5File(string md5file)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(FileMd5Data.Header);
        List<FileMd5Data> datas = GetAllValues();
        datas.Sort((a, b) => { return a.Id.CompareTo(b.Id); });
        foreach (var data in datas)
        {
            builder.AppendLine(data.ToString());
        }
        FileUtils.WritePersistentFile(builder.ToString(), md5file);
    }
}

