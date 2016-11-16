using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class ActionExtensions
{
    public static void Dispatch(this Action action)
    {
        if (action != null)
        {
            action.Invoke();
        }
    }

    public static void Dispatch<T>(this Action<T> action, T arg)
    {
        if(action != null)
        {
            action.Invoke(arg);
        }
    }
}

