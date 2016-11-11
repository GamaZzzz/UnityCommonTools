using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIMenuItem : UIToggle
{
    [SerializeField]
    private UIText menuName;

    public string MenuName
    {
        get
        {
            return menuName.Text;
        }
        set
        {
            menuName.Text = value;
        }
    }

    public int MenuIndex
    {
        get;set;
    }
}

