using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIColorItem : UIMenuItem
{
    public Color Color = Color.white;

    #if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        Image[] images = GetComponentsInChildren<Image>();
        foreach(var item in images)
        {
            item.color = Color;
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        Image[] images = GetComponentsInChildren<Image>();
        foreach (var item in images)
        {
            item.color = Color;
        }
    }
#endif
}

