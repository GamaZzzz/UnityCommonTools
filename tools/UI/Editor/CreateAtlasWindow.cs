#if UNITY_EDITOR
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
        public SpriteData(string name, Sprite sprite)
        {
            this.Sprite = sprite;
            this.Name = name;
        }
        public string Name;
        public Sprite Sprite;
        public bool IsChange = false;
    }

    [MenuItem("ExtensionTools/Open Atlas Window")]
    static void Init()
    {
        CreateAtlasWindow window = GetWindow<CreateAtlasWindow>();
    }

    public UIAtlas uiAtlas = null;
    private Dictionary<string, SpriteData> sprites = new Dictionary<string, SpriteData>();
    private Vector2 pos;
    private bool change = false;

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        float posX = 10;
        float posY = 10;

        Rect rect = new Rect(posX, posY, 500, 20);
        uiAtlas = EditorGUI.ObjectField(rect, "Atlas", uiAtlas, typeof(UIAtlas), false) as UIAtlas;

        if (uiAtlas)
        {
            Dictionary<string, Sprite> cur = uiAtlas.GetSprites();
            foreach(var sprite in cur.Values)
            {
                if (!sprites.ContainsKey(sprite.name))
                {
                    sprites.Add(sprite.name, new SpriteData(sprite.name, sprite));
                }
            }

            if (Selection.activeObject)
            {
                Sprite sprite = Selection.activeObject as Sprite;
                if (sprite && !sprites.ContainsKey(sprite.name))
                {
                    sprites.Add(sprite.name, new SpriteData(sprite.name, sprite));
                }
            }
        }
        else
        {
            sprites.Clear();
        }

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
        List<SpriteData> tempSprites = new List<SpriteData>(sprites.Values);
        List<string> deletes = new List<string>();
        foreach (var sp in tempSprites)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Sprite:", sp.Name, GUILayout.Width(500));
            if (GUILayout.Button("Delete"))
            {
                if (sprites.ContainsKey(sp.Name))
                {
                    deletes.Add(sp.Name);

                    DeleteSprite(sp.Name);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        foreach(var item in deletes)
        {
            sprites.Remove(item);
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
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Add/Update", style);
        }
        EditorGUILayout.EndVertical();

        this.Repaint();
    }

    void AddOrUpdateAtlas(string name)
    {
        GameObject newPrefabs = new GameObject(name);
        UIAtlas atlas = newPrefabs.AddComponent<UIAtlas>();

        foreach(var sp in sprites.Values)
        {
            atlas.AddSprite(sp.Sprite);
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

    void DeleteSprite(string name)
    {
        GameObject temp = Instantiate(uiAtlas.gameObject) as GameObject;
        UIAtlas atlas = temp.GetComponent<UIAtlas>();
        atlas.RemoveSprite(name);
        GameObject go = PrefabUtility.ReplacePrefab(temp, uiAtlas, ReplacePrefabOptions.Default);
        this.uiAtlas = go.GetComponent<UIAtlas>();
        DestroyImmediate(temp);
    }
}

#endif