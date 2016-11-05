using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BaseControll : MonoBehaviour
{
    void Update()
    {
#if UNITY_ANDROID
        // On Android, the Back button is mapped to the Esc key
        if (Input.GetKeyUp(KeyCode.Escape))
        {
#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
            Application.Quit();
#else // UNITY_5_3 or above
            Application.Quit();
#endif
        }
#endif
    }
}

