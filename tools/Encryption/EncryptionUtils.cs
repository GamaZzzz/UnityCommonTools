using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using UnityEngine;
using System.Collections;

public class EncryptionUtils : MonoBehaviour
{
    private static string server_access_key = "ce808ab28accb3e7eae331b574effb67d5f78a71";
    private static string Authorization = "VWS {0}:{1}";
    public static string Encrypt_HMACSHA1(string key, byte[] data)
    {
        HMACSHA1 hmascha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key));
        ToBase64Transform tbt = new ToBase64Transform();
        byte[] encrypteddata = hmascha1.ComputeHash(data);
        return Convert.ToBase64String(encrypteddata);
    }

    void Start()
    {
        StartCoroutine(TestVWS());
    }

    public IEnumerator TestVWS()
    {
        string date = DateTime.UtcNow.ToString();
        string url = "http://cloudreco.vuforia.com/v1/query";

        string StringToSign = "POST\n{0}\nmultipart/form-data\n{1}\n{2}";

        string Request_Body = "?image={0}&max_num_results=10&include_target_data=all";

        FileStream fs = File.OpenRead("e:\\car0_scaled.jpg");
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, buffer.Length);

        Request_Body = string.Format(Request_Body, buffer);
        byte[] postData = Encoding.UTF8.GetBytes(Request_Body);
        byte[] contentmd5 = MD5.Create().ComputeHash(postData);

        StringToSign = string.Format(StringToSign, contentmd5, date, url);
        string Signature = Encrypt_HMACSHA1(server_access_key, Encoding.UTF8.GetBytes(StringToSign));

        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add(HttpRequestHeader.Authorization.ToString(), string.Format(Authorization, server_access_key, Signature));
        headers.Add("content-type", "multipart/form-data");
        headers.Add(HttpRequestHeader.Accept.ToString(), "application/json");

        WWW www = new WWW(url, postData, headers);

        yield return www;

        if(www.isDone && string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.text);
        }
        else
        {
            Debug.Log(www.error);
        }
    }
}

