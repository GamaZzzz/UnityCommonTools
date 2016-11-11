using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ObjectBuilder
{
    public abstract T Build<T>(GameObject original) where T : MonoBehaviour;
    public abstract void Destroy<T>(T t) where T : MonoBehaviour;
}

