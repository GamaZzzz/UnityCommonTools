using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIAtlas : MonoBehaviour
{
    private Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
    private bool isInit = false;
    void Awake()
    {
        InitSprites();
    }

    void InitSprites()
    {
        if (!isInit)
        {
            SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
            foreach (var render in renders)
            {
                if (!spriteDictionary.ContainsKey(render.name))
                {
                    spriteDictionary.Add(render.sprite.name, render.sprite);
                }
            }
            isInit = true;
        }
    }

    public Sprite Load(string spriteName)
    {
        InitSprites();
        if (spriteDictionary.ContainsKey(spriteName))
        {
            return spriteDictionary[spriteName];
        }
        else
        {
            Debug.LogError("Sprite[" + spriteName + "] do not exist!");
        }
        return null;
    }

    public bool HasSprite(string spriteName)
    {
        InitSprites();
        return spriteDictionary.ContainsKey(spriteName);
    }

#if UNITY_EDITOR
    public void AddSprite(Sprite sprite)
    {
        GameObject go = new GameObject(sprite.name);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        go.transform.parent = transform;
        go.transform.ResetLocal();
    }

    public Dictionary<string, Sprite> GetSprites() {

        Dictionary<string, Sprite> ret = new Dictionary<string, Sprite>();

        SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in renders)
        {
            if (!ret.ContainsKey(render.name))
            {
                ret.Add(render.sprite.name, render.sprite);
            }
        }
        return ret;
    }

    public void RemoveSprite(string name)
    {
        SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in renders)
        {
            if(render.name == name)
            {
                DestroyImmediate(render.gameObject);
            }
        }
    }
#endif
}

