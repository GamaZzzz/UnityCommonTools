using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class GridItem : Widget
{
    public int Index { get; set; }

    public void SetOrder(int index)
    {
        RectRoot.SetSiblingIndex(index);
    }
}

