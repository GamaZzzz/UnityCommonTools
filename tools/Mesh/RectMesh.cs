using UnityEngine;

using DG.Tweening;

public class RectMesh : PrimitiveBase

{

    [SerializeField]

    private float m_Width;

    [SerializeField]

    private float m_Length;

    public enum PivotAlign { Left, Center, Right }

    public PivotAlign widthAlign = PivotAlign.Center;//宽度方向的中心点

    public PivotAlign lengthAlign = PivotAlign.Center;//长度方向的中心点

    /// <summary>

    /// 宽度

    /// </summary>

    public float width

    {

        get { return m_Width; }

        set { m_Width = value; UpdateShape(); }

    }

    /// <summary>

    /// 长度

    /// </summary>

    public float length

    {

        get { return m_Length; }

        set { m_Length = value; UpdateShape(); }

    }

    /// <summary>

    /// x方向的偏移

    /// </summary>

    public float offsetX

    {

        get { return offset.x; }

        set { offset.x = value; UpdateShape(); }

    }

    protected override void UpdateShape()

    {

        if (cacheTransform == null || meshFilter == null)

        {

            Init();

        }



        Vector3 localPos = offset;

        float w2 = m_Width * 0.5f;

        float l2 = m_Length * 0.5f;



        vertices = new Vector3[4];



        float x0, z0, x1, z1, x2, z2, x3, z3;

        x0 = z0 = x1 = z1 = x2 = z2 = x3 = z3 = 0;



        switch (widthAlign)

        {

            case PivotAlign.Center:

                x0 = -w2; x1 = w2; x2 = -w2; x3 = w2;

                break;

            case PivotAlign.Left:

                x0 = 0f; x1 = m_Width; x2 = 0f; x3 = m_Width;

                break;

            case PivotAlign.Right:

                x0 = -m_Width; x1 = 0f; x2 = -m_Width; x3 = 0f;

                break;

        }

        switch (lengthAlign)

        {

            case PivotAlign.Center:

                z0 = -l2; z1 = -l2; z2 = l2; z3 = l2;

                break;

            case PivotAlign.Left:

                z0 = 0f; z1 = 0; z2 = m_Length; z3 = m_Length;

                break;

            case PivotAlign.Right:

                z0 = -m_Length; z1 = -m_Length; z2 = 0f; z3 = 0f;

                break;

        }



        vertices[0].x = localPos.x + x0;

        vertices[0].y = localPos.y;

        vertices[0].z = localPos.z + z0;



        vertices[1].x = localPos.x + x1;

        vertices[1].y = localPos.y;

        vertices[1].z = localPos.z + z1;



        vertices[2].x = localPos.x + x2;

        vertices[2].y = localPos.y;

        vertices[2].z = localPos.z + z2;



        vertices[3].x = localPos.x + x3;

        vertices[3].y = localPos.y;

        vertices[3].z = localPos.z + z3;



        UpdateMesh();

    }

    protected override void InitMesh()

    {

        if (cacheTransform == null || meshFilter == null)

        {

            Init();

        }



        triangles = new int[] { 0, 2, 3, 0, 3, 1 };



        uvs = new Vector2[4];

        uvs[0].x = 0; uvs[0].y = 0;

        uvs[1].x = 1; uvs[1].y = 0;

        uvs[2].x = 0; uvs[2].y = 1;

        uvs[3].x = 1; uvs[3].y = 1;



        normals = new Vector3[4];

        normals[0].y = normals[1].y = normals[2].y = normals[3].y = 1;



        UpdateShape();

    }

    public Tweener DoLength(float endValue, float duration, float delay)

    {

        return DOTween.To(() => length, x => length = x, endValue, duration).SetDelay(delay);

    }

    public Tween DoWidth(float endValue, float duration, float delay)

    {

        return DOTween.To(() => width, x => width = x, endValue, duration).SetDelay(delay);

    }

}


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]

public class PrimitiveBase : MonoBehaviour
{

    /// <summary>

    /// 偏移

    /// </summary>

    public Vector3 offset;

    protected MeshFilter meshFilter;

    public Transform cacheTransform;

    void Awake()

    {

        Init();

        InitMesh();

    }

    protected void Init()

    {

        cacheTransform = transform;

        meshFilter = GetComponent<MeshFilter>();

        meshFilter.sharedMesh = new Mesh();

    }

    protected virtual void InitMesh()

    {



    }

    protected Vector3[] vertices;

    protected int[] triangles;

    protected Vector2[] uvs;

    protected Vector3[] normals;

    protected void UpdateMesh()

    {

        if (meshFilter.sharedMesh == null)

        {

            meshFilter.sharedMesh = new Mesh();

        }

        meshFilter.sharedMesh.vertices = vertices;

        meshFilter.sharedMesh.triangles = triangles;

        meshFilter.sharedMesh.uv = uvs;

        meshFilter.sharedMesh.normals = normals;

    }

    protected virtual void UpdateShape()

    {



    }

    void OnValidate()

    {

        InitMesh();

    }

}