﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using UnityEngine;

/// <summary>
/// CSV文件读取
/// </summary>
public class CSVFileReader : FileReader
{
    public List<string[]> Lines;

    public CSVFileReader(string sourcePath, string filePath) : base(sourcePath, filePath)
    {

    }

    protected override void OnCompleted(string data, byte[] bytes)
    {
        using(StringReader reader = new StringReader(data))
        {
            Lines = new List<string[]>();
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line))
            {
                Lines.Add(line.Split(','));
                line = reader.ReadLine();
            }
        }
        base.OnCompleted(data, bytes);
    }

    protected override void OnError(string error)
    {
        base.OnError(error);
    }

    public override IEnumerator DoAsync()
    {
        using (WWW www = new WWW(SourcePath + FilePath))
        {
            yield return www;

            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                OnCompleted(www.text, www.bytes);
            }
            else
            {
                DebugConsole.Error(www.error);
                OnError(www.error);
            }
        }
    }
}

