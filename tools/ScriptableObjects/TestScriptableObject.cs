using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu]
public class TestScriptableObject : ScriptableObject
{
    public List<Vector3> points;
    public GameObject[] prefabs;
}