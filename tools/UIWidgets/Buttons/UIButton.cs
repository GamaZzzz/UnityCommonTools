using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : Widget
{
    [SerializeField]
    private Button original;

    public event Action<UIButton> OnClick;

    void Awake()
    {
        if (original != null)
        {
            original.onClick.RemoveAllListeners();
            original.onClick.AddListener(On_Original_Clicked);
        }
    }

    void On_Original_Clicked()
    {
        OnClick.Dispatch(this);
    }
}