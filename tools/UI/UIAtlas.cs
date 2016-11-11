using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class UIAtlas : MonoBehaviour
{
    private Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    void Awake()
    {
        SpriteRenderer[] renders = GetComponentsInChildren<SpriteRenderer>();
        foreach(var render in renders)
        {
            spriteDictionary.Add(render.sprite.name, render.sprite);
        }
    }

    public Sprite Load(string spriteName)
    {
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
        return spriteDictionary.ContainsKey(spriteName);
    }

    public void AddSprite(Sprite sprite)
    {
        GameObject go = new GameObject(sprite.name);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        go.transform.parent = transform;
        go.transform.ResetLocal();
    }
}

