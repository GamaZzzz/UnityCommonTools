using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class DrawSpherePlane : MonoBehaviour
{

    public float Radius = 5;    //半径
    public int Segments = 50;   //分割数
    public float InnerRadius = 3;   //内圆半径
    public float AngleDegree = 90;

    private CircleMeshCreator _creator = new CircleMeshCreator();

    private MeshFilter _meshFilter;

    void Awake()
    {

        _meshFilter = GetComponent<MeshFilter>();
    }

    void Update()
    {
        _meshFilter.mesh = _creator.CreateMesh(Radius, Segments, InnerRadius, AngleDegree);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        DrawMesh();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        DrawMesh();
    }

    private void DrawMesh()
    {
        Mesh mesh = _creator.CreateMesh(Radius, Segments, InnerRadius, AngleDegree);
        int[] tris = mesh.triangles;
        for (int i = 0; i < tris.Length; i += 3)
        {
            Gizmos.DrawLine(TransformToWorld(mesh.vertices[tris[i]]), TransformToWorld(mesh.vertices[tris[i + 1]]));
            Gizmos.DrawLine(TransformToWorld(mesh.vertices[tris[i]]), TransformToWorld(mesh.vertices[tris[i + 2]]));
            Gizmos.DrawLine(TransformToWorld(mesh.vertices[tris[i + 1]]), TransformToWorld(mesh.vertices[tris[i + 2]]));
        }
    }

    private Vector3 TransformToWorld(Vector3 src)
    {
        return transform.TransformPoint(src);
    }

    private class CircleMeshCreator
    {
        private static readonly int PRECISION = 1000;
        private float _radius;
        private int _segments;
        private float _innerRadius;
        private float _angleDegree;

        private Mesh _cacheMesh;

        public Mesh CreateMesh(float radius, int segments, float innerRadius, float angleDegree)
        {
            if (checkDiff(radius, segments, innerRadius, angleDegree))
            {
                Mesh newMesh = Create(radius, segments, innerRadius, angleDegree);
                if (newMesh != null)
                {
                    _cacheMesh = newMesh;
                    this._radius = radius;
                    this._segments = segments;
                    this._innerRadius = innerRadius;
                    this._angleDegree = angleDegree;
                }
            }
            return _cacheMesh;
        }

        private Mesh Create(float radius, int segments, float innerRadius, float angleDegree)
        {

            if (segments <= 0)
            {
                segments = 1;
#if UNITY_EDITOR
                Debug.Log("segments must be larger than zero.");
#endif
            }

            Mesh mesh = new Mesh();
            int vlen = segments * 2 + 2;
            Vector3[] vertices = new Vector3[vlen];

            float angle = Mathf.Deg2Rad * angleDegree;
            float currAngle = angle / 2;
            float deltaAngle = angle / segments;
            for (int i = 0; i < vlen; i += 2)
            {
                float cosA = Mathf.Cos(currAngle);
                float sinA = Mathf.Sin(currAngle);
                vertices[i] = new Vector3(cosA * innerRadius, 0, sinA * innerRadius);
                vertices[i + 1] = new Vector3(cosA * radius, 0, sinA * radius);
                currAngle -= deltaAngle;
            }

            int tlen = segments * 6;
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

            Vector2[] uvs = new Vector2[vlen];
            for (int i = 0; i < vlen; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / radius / 2 + 0.5f, vertices[i].z / radius / 2 + 0.5f);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            return mesh;
        }

        private bool checkDiff(float radius, int segments, float innerRadius, float angleDegree)
        {
            return segments != this._segments || (int)((angleDegree - this._angleDegree) * PRECISION) != 0 ||
                (int)((radius - this._radius) * PRECISION) != 0 || (int)((innerRadius - this._innerRadius) * PRECISION) != 0;
        }
    }

}
