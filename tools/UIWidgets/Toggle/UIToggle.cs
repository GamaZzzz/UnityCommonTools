using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : Widget
{
    [SerializeField]
    private Toggle original;

    public Toggle Original { get { return original; } }

    public event Action<UIToggle> OnToggle;

    protected virtual void Awake()
    {
        original.onValueChanged.RemoveAllListeners();
        original.onValueChanged.AddListener(On_ToggleValue_Changed);
    }

    void On_ToggleValue_Changed(bool isOn)
    {
        OnToggle.Dispatch(this);
    }

    public bool Value
    {
        get
        {
            return original.isOn;
        }
        set
        {
            original.isOn = value;
        }
    }


#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        original = GetComponent<Toggle>();
    }

    protected override void OnValidate()
    {

    }
#endif
}

