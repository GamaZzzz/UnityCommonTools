﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UILabel : Widget
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}

