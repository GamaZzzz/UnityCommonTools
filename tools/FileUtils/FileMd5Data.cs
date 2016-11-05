using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class FileMd5Data : BaseData
{
    public static readonly string Header = "ID,FileName,FullPath,Md5Code";

    public int Id { get; set; }
    public string FileName { get; set; }
    public string FullPath { get; set; }
    public string Md5Code { get; set; }

    public override void Parse(string[] data)
    {
        Id = int.Parse(data[0].Trim());
        FileName = data[1].Trim();
        FullPath = data[2].Trim();
        Md5Code = data[3].Trim();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Id).Append(",")
            .Append(FileName).Append(",")
            .Append(FullPath).Append(",")
            .Append(Md5Code);
        return sb.ToString();
    }
}

