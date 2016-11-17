using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Widget : MonoBehaviour
{
    public RectTransform RectRoot;

    public virtual void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public void Show()
    {
        Show(true);
    }

    public void Hide()
    {
        Show(false);
    }
#if UNITY_EDITOR
    protected virtual void Reset()
    {
        RectRoot = GetComponent<RectTransform>();
    }

    protected virtual void OnValidate()
    {

    }
#endif
}

