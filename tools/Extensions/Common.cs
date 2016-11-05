using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Common
{
    public static void ResetLocal(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    public static string ListToString<T>(this List<T> list)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        foreach (var item in list)
        {
            sb.Append(item.ToString()).Append(",");
        }
        sb.Append("]");
        return sb.ToString();
    }
}
