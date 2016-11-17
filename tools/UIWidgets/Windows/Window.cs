using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class Window : Widget
{
    public event Action<Window> OnOpened;
    public event Action<Window> OnClosed;

    private bool isOpened = false;

#if UNITY_EDITOR 
    [ContextMenu("Parse Sub Widgets as Field of this Class")]
    public void ParseSubWidgetsAsField()
    {
        string filename = this.GetType().Name + ".cs";
        string dir = Environment.CurrentDirectory;
        string find = FileUtils.GetFileDir(dir, filename);
        string text = FileUtils.ReadFile(find);

        
        StringBuilder sb = new StringBuilder();
        int index = text.IndexOf("#region generated");
        int insert = index + 1;
        bool isregion = index > 0;
        string restr = "";
        if (index < 0)
        {
            index = text.IndexOf("public") + 1;
            insert = text.IndexOf("{", index) + 1;
            sb.Append("#region generated");
        }
        else
        {
            int reindex = text.IndexOf("#endregion");
            restr = text.Substring(index + 17, reindex - index - 17);
        }
        Widget[] widgets = this.GetComponentsInChildren<Widget>();
        foreach(var widget in widgets)
        {
            if(widget.transform.parent == this.transform)
            {
                sb.Append("\r\n\t\t")
                .Append("public\t")
                .Append(widget.GetType().Name)
                .Append("\t")
                .Append(widget.gameObject.name)
                .Append(";");
            }   
        }
        if (!isregion)
        {
            sb.Append("\r\n\t\t#endregion");
            text = text.Insert(insert, sb.ToString());
        }
        else
        {
            sb.Append("\r\n\t\t");
            text = text.Replace(restr, sb.ToString());
        }
        FileUtils.WritePersistentFile(text, find);
    }
#endif

    public virtual void Open()
    {
        if (!isOpened)
        {
            Show();
            OnOpened.Dispatch(this);
            isOpened = true;
        }
    }

    public virtual void Close()
    {
        if (isOpened)
        {
            Hide();
            OnClosed.Dispatch(this);
            isOpened = false;
        }
    }
}

