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
        if (action != null)
        {
            action.Invoke(arg);
        }
    }

    public static void Dispatch<T1, T2>(this Action<T1, T2> action, T1 arg0, T2 arg1)
    {
        if (action != null)
        {
            action.Invoke(arg0, arg1);
        }
    }

    public static void Dispatch<T1, T2, T3>(this Action<T1, T2, T3> action, T1 arg0, T2 arg1, T3 arg2)
    {
        if (action != null)
        {
            action.Invoke(arg0, arg1, arg2);
        }
    }
}

