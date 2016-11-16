using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class LoadingScreen : Widget
{
    public abstract void OnProgress(float progress);
}

