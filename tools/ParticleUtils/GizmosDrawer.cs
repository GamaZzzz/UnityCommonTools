using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class GizmosDrawer : MonoBehaviour
{
    List<Vector3> nodes = new List<Vector3>();
    [ExecuteInEditMode]
    void OnDrawGizmos()
    {
        int count = nodes.Count;
        for (int i = 1; i < count; ++i)
        {
            Gizmos.DrawLine(nodes[i - 1], nodes[i]);
        }
    }

    public void SetNodes(List<Vector3> nodes)
    {
        this.nodes.Clear();
        this.nodes.AddRange(nodes);
    }
}

