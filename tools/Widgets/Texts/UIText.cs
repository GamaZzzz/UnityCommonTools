using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIText : Widget
{
    [SerializeField]
    private Text original;

    public string Text
    {
        get { return original.text; }
        set
        {
            original.text = value;
        }
    }

    public void SetText(object text)
    {
        original.text = text.ToString();
    }

    public int IntValue
    {
        get
        {
            if(original != null)
            {
                return int.Parse(original.text.Trim());
            }
            return 0;
        }
    }
}

