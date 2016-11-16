using UnityEngine;
using UnityEngine.Sprites;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Sprites;

[InitializeOnLoad]
public static class EditorTools {

    [MenuItem("ExtensionTools/Write Prefab Path to C#", validate = false)]
    public static void WritePrefabPathToCSharp()
    {
       Debug.Log(string.Join(",",Packer.atlasNames));
       Texture2D[] texts =  Packer.GetTexturesForAtlas("1");
        Sprite sprite0 = Resources.Load<Sprite>("Sprites/car02");
        Sprite sprite1 = Resources.Load<Sprite>("Sprites/car0_scaled");
        Vector2[] points0 = SpriteUtility.GetSpriteUVs(sprite0, true);
        Vector2[] points1 = SpriteUtility.GetSpriteUVs(sprite1, true);
        
        Debug.Log("tEXT");
    }

    [MenuItem("ExtensionTools/Generate StreamAssets Files Md5 Ver", validate = false)]
    public static void GenerateStreamAssetsFileMd5Ver()
    {
        List<string> ret = FileUtils.GetAllFileFullPathsInStreamingAssets(Application.dataPath + "/StreamingAssets");
        Debug.Log(ret.ListToString());
        FileMd5DataDictionary dic = new FileMd5DataDictionary();
        int i = 1;
        foreach(var file in ret)
        {
            if (!file.Contains(".meta") && !file.Contains("FilesMd5"))
            {
                FileMd5Data data = new FileMd5Data();
                data.Id = i;
                int spliteIndex = file.IndexOf("StreamingAssets") + "StreamingAssets".Length + 1;
                string subFileName = file.Substring(spliteIndex).Replace("\\","/");
                data.FullPath = subFileName;
                
                spliteIndex = subFileName.LastIndexOf('.');
                subFileName = subFileName.Substring(0, spliteIndex);
                spliteIndex = subFileName.LastIndexOf('/');
                if(spliteIndex >= 0)
                {
                    subFileName = subFileName.Substring(spliteIndex+1);
                }
                data.FileName = subFileName;
                if (data.FullPath.Contains(".dat"))
                {
                    data.FileName += ".dat";
                }
                
                data.Md5Code = FileUtils.GetMd5_Path(file);
               
                dic.AddData(data);
                i++;
            }
        }
        dic.UpdateMd5File(Application.dataPath + "/StreamingAssets/FilesMd5.csv");
    }
}
