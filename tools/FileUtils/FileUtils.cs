using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FileUtils
{
    public static List<string>  GetAllFileFullPathsInStreamingAssets(string dir)
    {
        List<string> ret = new List<string>();
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        ret.AddRange(GetAllFileFullPathsInStreamingAssets(dirInfo));
        return ret;
    }

    public static List<string> GetAllFileFullPathsInStreamingAssets(DirectoryInfo dir)
    {
        List<string> ret = new List<string>();
        FileInfo[] files = dir.GetFiles();
        foreach(var fl in files)
        {
            ret.Add(fl.FullName);
        }
        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach(var di in dirs)
        {
            ret.AddRange(GetAllFileFullPathsInStreamingAssets(di));
        }
        return ret;
    }

    public static string GetFileDir(string dir, string filename)
    {
        string ret = "";
        DirectoryInfo dirInfo = new DirectoryInfo(dir);
        ret = GetFileDir(dirInfo, filename);
        return ret;
    }

    public static string GetFileDir(DirectoryInfo dir, string filename)
    {
        string ret = "";

        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (var item in dirs)
        {
            FileInfo[] files = item.GetFiles();
            foreach (var file in files)
            {
                if (file.Name == filename)
                {
                    return file.FullName;
                }
            }
            ret = GetFileDir(item, filename);
            if(ret != "")
            {
                return ret;
            }
        }
        return ret;
    }

    public static void CopyFileAndDirectories(string src, string dest)
    {
        
    }

    public static void CopyFileAndDirectories(DirectoryInfo dir, string dest)
    {
        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (var item in dirs)
        {
            FileInfo[] files = item.GetFiles();
            foreach (var file in files)
            {
                
            }
           CopyFileAndDirectories(item, dest);
        }
    }

    public static string ReadFile(string fullname)
    {
        string text = "";
        using (StreamReader reader = File.OpenText(fullname))
        {
            text = reader.ReadToEnd();
        }
        return text;
    }

    public static void WritePersistentFile(string content, string persistentFile)
    {
        if (!File.Exists(persistentFile))
        {
            DirectoryInfo di = new DirectoryInfo(persistentFile);
            if (!di.Parent.Exists)
            {
                di.Parent.Create();
#if UNITY_EDITOR
                Debug.Log("Create Dir:"+ di.Parent.FullName);
#else
                DebugConsole.Info("Create Dir:"+ di.Parent.FullName);
#endif
            }

            FileStream file = File.Create(persistentFile);
            file.Close();
            file.Dispose();
            file = null;
        }

        FileStream writer = File.OpenWrite(persistentFile);

        using (StreamWriter sw = new StreamWriter(writer))
        {
            writer.SetLength(0);
            sw.Write(content);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        writer.Close();
        writer.Dispose();

        writer = null;
    }


    public static string GetMd5(string content)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] output = md5.ComputeHash(Encoding.Unicode.GetBytes(content));
        string md5str = BitConverter.ToString(output).Replace("-", "");
        return md5str;
    }

    public static string GetMd5_Path(string filepath)
    {
        string md5str = "";
        using (FileStream stream = File.Open(filepath, FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(stream);
            md5str = BitConverter.ToString(output).Replace("-", "");
        }
        return md5str;
    }

    public static bool CheckFileVersion(string path, string lastVerMd5)
    {
        bool ret = false;
        using (FileStream stream = File.Open(path, FileMode.Open))
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(stream);
            string md5str = BitConverter.ToString(output).Replace("-", "");
            ret = md5str.CompareTo(lastVerMd5) == 0;
        }
        return ret;
    }
}

