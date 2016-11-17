using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class UIColorSelector : UIMenu
{
    public event Action<UIColorItem> OnColorSelected;

    protected override void Start()
    {
        base.Start();
        OnMenuClicked -= On_Color_Selected;
        OnMenuClicked += On_Color_Selected;
    }

    void On_Color_Selected(UIMenuItem item)
    {
        OnColorSelected.Dispatch(item as UIColorItem);
    }

    protected override void OnDestroy()
    {
        OnMenuClicked -= On_Color_Selected;
        base.OnDestroy();
    }
}

