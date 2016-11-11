using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
[RequireComponent(typeof(GridLayoutGroup))]
public class UIMenu : Widget
{
    [SerializeField]
    private int defaultIndex = 0;

    private UIMenuItem[] menuItems;
    private ToggleGroup group;
    private GridLayoutGroup grid;

    public event Action<UIMenuItem> OnMenuClicked;

    void Start()
    {
        group = GetComponent<ToggleGroup>();
        grid = GetComponent<GridLayoutGroup>();
        menuItems = GetComponentsInChildren<UIMenuItem>(true);
        if (menuItems != null && menuItems.Length > 0)
        {
            int count = menuItems.Length;
            for (int i=0; i< count; i++)
            {
                menuItems[i].OnToggle -= On_MenuItem_Clicked;
                menuItems[i].OnToggle += On_MenuItem_Clicked;
                menuItems[i].MenuIndex = i;
                if (i == defaultIndex)
                {
                    group.NotifyToggleOn(menuItems[i].Original);
                }
            }
        }
    }

    void On_MenuItem_Clicked(UIToggle toggle)
    {
        OnMenuClicked.Dispatch(toggle as UIMenuItem);
    }
}

