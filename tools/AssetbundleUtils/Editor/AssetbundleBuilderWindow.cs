using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetbundleBuilderWindow : EditorWindow
{
    [MenuItem("ExtensionTools/AssetbundleBuilder Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AssetbundleBuilderWindow));
    }

    string path;
    Rect rect;
    Vector2 scrollRect;

    void OnEnable()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
            Debug.Log("AssetBundle: " + name);
    }
    string tempPath = "";
    void OnGUI()
    {
        EditorGUILayout.Space();

        #region Dire
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("路径");
        //获得一个长300的框  
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(450));
        //将上面的框作为文本输入框  
        path = EditorGUI.TextField(rect, path);
        //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  
        if (Event.current.type == EventType.DragUpdated)
        {
            //改变鼠标的外表  
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    tempPath = DragAndDrop.paths[0];
                }
                else
                {
                    tempPath = "";
                }
            }
        }
        else if (Event.current.type == EventType.DragExited)
        {
            Debug.Log("DragExited:" + tempPath);

            DirectoryInfo dire = new DirectoryInfo(tempPath);
            if (dire.Exists)
            {
                path = tempPath;
            }
        }

        EditorGUILayout.EndHorizontal();
        #endregion
        EditorGUILayout.Space();
        #region scrollveiw
        using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollRect))
        {
            scrollRect = scrollViewScope.scrollPosition;
            scrollViewScope.handleScrollWheel = true;
            var names = AssetDatabase.GetAllAssetBundleNames();
            
            foreach (var name in names)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("名称:");
                Rect newRect = EditorGUILayout.GetControlRect(GUILayout.Width(450));
                //将上面的框作为文本输入框  
                EditorGUI.TextField(newRect, name);
                EditorGUILayout.EndHorizontal();
            }
        }
        #endregion
        EditorGUILayout.Space();
        #region
        using (var h = new EditorGUILayout.HorizontalScope("Button"))
        {
            if (GUILayout.Button("Build Android"))
            {
                BuildAndroid(path);
            }

            if (GUILayout.Button("Build IOS"))
            {

            }
        }
        #endregion

        this.Repaint();
    }

    void BuildAndroid(string path)
    {

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    void BuildIOS(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.iOS);
    }
}

public class MyPostprocessor : AssetPostprocessor
{
    void OnPostprocessAssetbundleNameChanged(string path,
            string previous, string next)
    {
        //Debug.Log("AB: " + path + " old: " + previous + " new: " + next);
        //AssetbundleBuilderWindow abw = EditorWindow.GetWindow<AssetbundleBuilderWindow>(true);
        //if (abw.hideFlags != HideFlags.HideInHierarchy)
        //{
        //    abw.SendEvent(EditorGUIUtility.CommandEvent("ABChanged"));
        //}
    }
}

