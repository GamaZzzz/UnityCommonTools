using UnityEngine;
using System.Collections;
using System;

public class MeshUtils
{
    public Vector3[] Matrix4x4_Translate(Vector3 translation,
     Vector3 eulerAngles,
     Vector3 scale,
     Vector3[] origVerts)
    {
        Vector3[] newVerts = new Vector3[origVerts.Length];
        Quaternion rotation = Quaternion.Euler(eulerAngles);
        Matrix4x4 m = Matrix4x4.identity;

        m.SetTRS(translation, rotation, scale);
        int i = 0;
        while (i < origVerts.Length)
        {
            newVerts[i] = m.MultiplyPoint3x4(origVerts[i]);
            i++;
        }
        return newVerts;
    }

    public void CombineMesh(Transform target)
    {
        var meshFilter = target.GetComponent<MeshFilter>();
        if (meshFilter && target.GetComponent<Renderer>())
        {
            var children = target.GetComponentsInChildren<MeshFilter>();
            var combine = new CombineInstance[children.Length + 1];
            combine[0].mesh = meshFilter.sharedMesh;
        }
    }
}
