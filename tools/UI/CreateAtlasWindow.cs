using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CreateAtlasWindow : EditorWindow
{
    class SpriteData
    {
        public Sprite Sprite;
        public bool IsChange = false;
    }

    [MenuItem("ExtensionTools/Open Atlas Window")]
    static void Init()
    {
        CreateAtlasWindow window = GetWindow<CreateAtlasWindow>();
    }

    public UIAtlas uiAtlas = null;
    private List<Sprite> sprites = new List<Sprite>();
    private Vector2 pos;
    private bool change = false;

    void OnGUI()
    {
        if (Selection.activeObject)
        {         
            Sprite sprite = Selection.activeObject as Sprite;
            if(sprite && !sprites.Contains(sprite))
            {
                sprites.Add(sprite);
            }
        }

        EditorGUILayout.BeginVertical();

        float posX = 10;
        float posY = 10;

        Rect rect = new Rect(posX, posY, 500, 20);
        uiAtlas = EditorGUI.ObjectField(rect, "Atlas", uiAtlas, typeof(UIAtlas), false) as UIAtlas;
        posY += 20;
        rect = new Rect(posX, posY, 100, 50);
        GUILayout.BeginArea(rect);
        using (var h = new EditorGUILayout.HorizontalScope("Button"))
        {
            if (GUI.Button(h.rect, GUIContent.none))
            {
                CreateNewAtlas();
            }
            GUILayout.Label("New Atlas");
        }
        GUILayout.EndArea();

        posY += 50;

        pos = EditorGUILayout.BeginScrollView(new Vector2(posX, posY));

        GUILayout.BeginArea(new Rect(pos, new Vector2(600, 250)));

        EditorGUILayout.BeginVertical();

        foreach (var sp in sprites)
        {
            EditorGUILayout.TextField("Sprite:", sp.name, GUILayout.Width(500));
        }

        EditorGUILayout.EndVertical();

        GUILayout.EndArea();
        EditorGUILayout.EndScrollView();

        using (var h = new EditorGUILayout.HorizontalScope("Button"))
        {
            if (GUI.Button(h.rect, GUIContent.none))
            {
                AddOrUpdateAtlas(uiAtlas.name);
            }
            GUILayout.Label("Add/Update");
        }
        EditorGUILayout.EndVertical();
    }

    void AddOrUpdateAtlas(string name)
    {
        GameObject newPrefabs = new GameObject(name);
        UIAtlas atlas = newPrefabs.AddComponent<UIAtlas>();

        foreach(var sp in sprites)
        {
            atlas.AddSprite(sp);
        }

        GameObject go = PrefabUtility.ReplacePrefab(newPrefabs, uiAtlas, ReplacePrefabOptions.Default);

        uiAtlas = go.GetComponent<UIAtlas>();

        DestroyImmediate(newPrefabs);
    }

    void CreateNewAtlas()
    {
        GameObject go = new GameObject("New Atlas");
        UIAtlas uiAtlas = go.AddComponent<UIAtlas>();
        this.uiAtlas = uiAtlas;
    }
}

