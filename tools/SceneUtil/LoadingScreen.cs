using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class ALoadingScreen : Widget
{
    public abstract void OnProgress(float progress);
}

