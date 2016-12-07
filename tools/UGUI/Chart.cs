using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Chart : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private PathParticle pathParticle;
    [SerializeField]
    private Transform original;
    [SerializeField]
    private float axisX = 100f;
    [SerializeField]
    private float axisY = 100f;
    [SerializeField]
    private float scaleX = 1f;
    [SerializeField]
    private float scaleY = 1f;
    [SerializeField]
    private List<Vector2> frameKeys;
    [SerializeField]
    private int segments = 50;

    private List<LineRenderer> lines = new List<LineRenderer>();

    void Start()
    {
        if (!pathParticle)
        {
            pathParticle = GetComponent<PathParticle>();
        }
        DrawChartLine(frameKeys);
    }

    void DrawChart(List<Vector2> frameKeys)
    {
        if (original && frameKeys.Count > 1)
        {
            int count = frameKeys.Count;
            Vector3[] positons = new Vector3[count + 1];
            Vector3 org = original.position;
            positons[0].Set(org.x, org.y, org.z);
            for (int i = 1, j = 0; i < count + 1 && j < count; ++j, ++i)
            {
                positons[i].x = org.x + frameKeys[j].x;
                positons[i].y = org.y + frameKeys[j].y;
                positons[i].z = org.z;
            }
            pathParticle.DrawPath(new List<Vector3>(positons));
        }
    }

    void DrawChartMsf(List<Vector2> frameKeys)
    {
        if (original && frameKeys.Count > 1)
        {
            int count = frameKeys.Count;
            Vector3[] positons = new Vector3[count + 1];
            Vector3 org = original.position;
            positons[0].Set(org.x, org.y, org.z);
            for (int i = 1, j = 0; i < count + 1 && j < count; ++j, ++i)
            {
                positons[i].x = org.x + frameKeys[j].x;
                positons[i].y = org.y + frameKeys[j].y;
                positons[i].z = org.z;
            }
            Vector3[] positons_beta = new Vector3[count + 1];
            for (int i = 0; i < count + 1; ++i)
            {
                
                positons_beta[i].x = positons[i].x;
                positons_beta[i].y = positons[i].y;
                positons_beta[i].z = positons[i].z;

            }

            for (int i=0; i< count + 1; ++i)
            {
                //float angle = angle_360(Vector3.right, positons_beta[i-1] - positons[i]);

                if (i == 0)
                {
                    positons_beta[i].y -= 0.1f;
                }
                else if (i == count)
                {
                    positons_beta[i].x += 0.1f;
                }
                else
                {
                    positons_beta[i].y -= 0.1f;
                    positons_beta[i].x += 0.1f;
                }
            }

            int vlen = 2 * (count + 1);
            Vector3[] vertices = new Vector3[vlen];
            for (int i = 0, j = 0; i < vlen && j < count + 1; i += 2, j++)
            {
                vertices[i] = positons[j];
                vertices[i + 1] = positons_beta[j];
            }

            int tlen = count * 6;
            int[] triangles = new int[tlen];
            for (int i = 0, vi = 0; i < tlen; i += 6, vi += 2)
            {
                triangles[i] = vi;
                triangles[i + 1] = vi + 1;
                triangles[i + 2] = vi + 3;
                triangles[i + 3] = vi + 3;
                triangles[i + 4] = vi + 2;
                triangles[i + 5] = vi;
            }
            float radius = 0.3f;
            Vector2[] uvs = new Vector2[vlen];
            for (int i = 0; i < vlen; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / radius / 2 + 0.5f, vertices[i].z / radius / 2 + 0.5f);
            }
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.triangles = triangles;
            meshFilter.mesh.uv = uvs;
        }
    }

    [SerializeField]
    private LineRenderer line;
    void DrawChartLine(List<Vector2> frameKeys)
    {
        if (original && frameKeys.Count > 1)
        {
            int count = frameKeys.Count;
            Vector3[] positons = new Vector3[count + 1];
            Vector3 org = original.position;
            positons[0].Set(org.x, org.y, org.z);
            for (int i = 1, j = 0; i < count + 1 && j < count; ++j, ++i)
            {
                positons[i].x = org.x + frameKeys[j].x;
                positons[i].y = org.y + frameKeys[j].y;
                positons[i].z = org.z;
            }
            for(int i=0; i< count; ++i)
            {
                if(lines.Count > i)
                {
                    lines[i].SetPosition(0, positons[i]);
                    lines[i].SetPosition(1, positons[i+1]);
                }
                else
                {
                    GameObject go = new GameObject("Node" + i);
                    LineRenderer line = go.AddComponent<LineRenderer>();
                    line.SetWidth(0.01f, 0.01f);
                    go.transform.parent = transform;
                    lines.Add(line);
                    line.SetVertexCount(2);
                    line.SetPosition(0, positons[i]);
                    line.SetPosition(1, positons[i + 1]);
                }
            }
        }
    }

    float angle_360(Vector3 from_, Vector3 to_)
    {
        float angle = 0f;
        Vector3 v3 = Vector3.Cross(from_, to_);

        if (v3.z > 0)

            angle = Vector3.Angle(from_, to_);

        else

            angle = 360 - Vector3.Angle(from_, to_);

        return Mathf.PI * 2 * angle / 360;
    }

#if UNITY_EDITOR
    void OnDestroy()
    {
        for (int i = 0; i < lines.Count; ++i)
        {
            DestroyImmediate(lines[i].gameObject);
        }
        lines.Clear();
    }

    void Reset()
    {
        if (!pathParticle)
        {
            pathParticle = GetComponent<PathParticle>();
        }
    }
#endif
}

