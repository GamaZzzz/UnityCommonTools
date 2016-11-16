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
            this.SpriteName = name;
        }
        public string SpriteName;
        public Sprite Sprite;
        public bool IsChange = true;
    }

    [MenuItem("ExtensionTools/Open Atlas Window")]
    static void Init()
    {
        CreateAtlasWindow window = GetWindow<CreateAtlasWindow>();
    }

    public UIAtlas uiAtlas = null;
    private Dictionary<string,SpriteData> sprites = new Dictionary<string, SpriteData>();
    private Vector2 pos;
    void OnGUI()
    {
        if (Selection.activeObject)
        {         
            Sprite sprite = Selection.activeObject as Sprite;
            if(sprite)
            {
                if (!sprites.ContainsKey(sprite.name))
                {
                    sprites.Add(sprite.name, new SpriteData(sprite.name, sprite));
                }
                else
                {
                    if(sprites[sprite.name].Sprite != sprite)
                    {
                        sprites[sprite.name].Sprite = sprite;
                        sprites[sprite.name].IsChange = true;
                    }
                }
            }

        }

        EditorGUILayout.BeginVertical();

        float posX = 10;
        float posY = 10;

        Rect rect = new Rect(posX, posY, 500, 20);

        uiAtlas = EditorGUI.ObjectField(rect, new GUIContent("Atlas:"), uiAtlas, typeof(UIAtlas), false) as UIAtlas;

        if (uiAtlas)
        {
            foreach(var item in uiAtlas.GetSprites())
            {
                if (!sprites.ContainsKey(item.Key))
                {
                    sprites.Add(item.Key, new SpriteData(item.Key, item.Value));
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
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("New Atlas", style);
        }
        GUILayout.EndArea();

        posY += 50;

        pos = EditorGUILayout.BeginScrollView(new Vector2(posX, posY)); 

        GUILayout.BeginArea(new Rect(pos, new Vector2(600, 250)));
        
        EditorGUILayout.BeginVertical();

        List<string> curSprites = new List<string>(sprites.Keys);
        foreach (var sp in curSprites)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Sprite:", sp, GUILayout.Width(500));
            
            if(GUILayout.Button(new GUIContent("Delete")))
            {
                sprites.Remove(sp);
                DeleteSprite(sp);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        GUILayout.EndArea();

        EditorGUILayout.EndScrollView();

        using (var h = new EditorGUILayout.HorizontalScope("Button"))
        {
            if (GUI.Button(h.rect, GUIContent.none))
            {
                if (uiAtlas)
                {
                    AddOrUpdateAtlas(uiAtlas.name);
                }
                else
                {
                    Debug.LogError("Atlas can't be null!");
                }
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

        foreach(var sp in sprites)
        {
            atlas.AddSprite(sp.Value.Sprite);
        }

        GameObject go = PrefabUtility.ReplacePrefab(newPrefabs, uiAtlas, ReplacePrefabOptions.Default);

        uiAtlas = go.GetComponent<UIAtlas>();

        DestroyImmediate(newPrefabs);
    }

    void CreateNewAtlas()
    {
        GameObject go = new GameObject("New Atlas");
        UIAtlas uiAtlas = go.AddComponent<UIAtlas>();
        
        GameObject newGo = PrefabUtility.CreatePrefab("Assets/NewAtlas.prefab", go);
        DestroyImmediate(go);

        this.uiAtlas = newGo.GetComponent<UIAtlas>();
    }

    void DeleteSprite(string sprite)
    {
        Debug.Log("DeleteSprite");
        GameObject go = Instantiate(this.uiAtlas.gameObject);

        UIAtlas uia = go.GetComponent<UIAtlas>();
        if (uia.HasSprite(sprite))
        {
            uia.RemoveSprite(sprite);

            GameObject last = PrefabUtility.ReplacePrefab(go, uiAtlas, ReplacePrefabOptions.Default);

            this.uiAtlas = last.GetComponent<UIAtlas>();
        }

        DestroyImmediate(go);
    }
}

