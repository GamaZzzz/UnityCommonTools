using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class ParticlePathCurveMaker : EditorWindow
{
    public  class PathNode
    {
        public PathNode(int index)
        {
            Index = index;
            Node = null;
            Remove = false;
            Tip = null;
        }
        public int Index;
        public Transform Node;
        public bool Remove;
        public GameObject Tip;
        public void ShowTip()
        {
            if (Tip)
            {
                Tip.transform.parent = Node;
                Tip.transform.ResetLocal();
                TextMesh textMesh = Tip.GetComponent<TextMesh>();
                textMesh.text = "Node " + Index;
                Collider collider = Node.GetComponent<Collider>();
                if (collider)
                {
                    float h = collider.bounds.size.y;
                    Vector3 temp = Tip.transform.position;
                    temp.y += h;
                    Tip.transform.position = temp;
                }
            }
            else
            {
                GameObject tip = new GameObject("Node_" + Index);
                tip.AddComponent<MeshRenderer>();
                TextMesh textMesh = tip.AddComponent<TextMesh>();
                textMesh.text = "Node " + Index;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.color = Color.green;
                textMesh.fontSize = 10;
                Tip = tip;
                tip.transform.parent = Node;
                tip.transform.ResetLocal();
            }
        }
    }

    private ParticleSystem psystem;
    private Dictionary<int, PathNode> nodes = new Dictionary<int, PathNode>();
    private int curNodes = 0;
    private bool needResort = false;
    private Vector2 scrollPosition;
    private GizmosDrawer gd = null;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        if (!gd)
        {
            GameObject go = new GameObject("Gizmos");
            gd = go.AddComponent<GizmosDrawer>();
            if (!gd)
            {
                Debug.Log("Fiald");
            }
        }
    }

    void OnDisable()
    {
        foreach (var item in nodes.Values)
        {
            if (item.Tip)
            {
                DestroyImmediate(item.Tip);
            }
        }
        nodes.Clear();
        if (gd)
        {
            DestroyImmediate(gd.gameObject);
            gd = null;
        }
    }

    [MenuItem("ExtensionTools/Open Particle Path Curve Maker")]
    static void Init()
    {
        ParticlePathCurveMaker window = GetWindow<ParticlePathCurveMaker>();
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        Rect rect = GUILayoutUtility.GetRect(500f, 23f);
        psystem = EditorGUI.ObjectField(rect, "ParticleSystem:", psystem, typeof(ParticleSystem), true) as ParticleSystem;

        if (psystem)
        {
            ParticleSystem.ShapeModule shade = psystem.shape;
            shade.enabled = false;
            psystem.startSpeed = 0;
        }

        EditorGUILayout.Space();
        EditorGUILayout.PrefixLabel("Nodes:");
        List<Vector3> path = new List<Vector3>();
        using (var scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
        {
            scrollPosition = scrollViewScope.scrollPosition;
            scrollViewScope.handleScrollWheel = true;
            int count = nodes.Count;
            List<PathNode> temp = new List<PathNode>(nodes.Values);
            for (int i = 0; i < count; ++i)
            {
                MakePathNode(temp[i]);
            }
        }

        EditorGUILayout.Space();

        if (needResort)
        {
            ResortNodes();
        }

        List<Vector3> linePos = new List<Vector3>();
        foreach (var item in nodes.Values)
        {
            if (item.Node)
            {
                linePos.Add(item.Node.position);
            }
        }
        if (gd)
        {
            gd.SetNodes(linePos);
        }

        using (var h = new EditorGUILayout.HorizontalScope("Button"))
        {
            if (GUILayout.Button("Add path node"))
            {
                //添加路径点
                nodes.Add(curNodes,new PathNode(curNodes));
                curNodes++;
            }

            if(GUILayout.Button("Generate Curve"))
            {
                if (psystem && nodes.Count > 1)
                {
                    GenerateCurve();
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();

        this.Repaint();
    }

    void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }

    //显示路径备选单元格
    void MakePathNode(PathNode node)
    {
        EditorGUILayout.BeginHorizontal();
        Rect rect = GUILayoutUtility.GetRect(500f, 20f);
        node.Node = EditorGUI.ObjectField(rect, string.Format("Node {0}:",node.Index), node.Node, typeof(Transform), true) as Transform;

        if (node.Node)
        {
            node.ShowTip();
        }

        if (GUILayout.Button("Remove"))
        {
            node.Remove = true;
            nodes.Remove(node.Index);
            needResort = true;
            DestroyImmediate(node.Tip);
            node.Tip = null;
            --curNodes;
        }
        EditorGUILayout.EndHorizontal();
    }
    //重新排序路径点
    void ResortNodes()
    {
        List<PathNode> temp = new List<PathNode>(nodes.Values);
        temp.Sort((a, b) =>
        {
            return a.Index.CompareTo(b.Index);
        });
        nodes.Clear();
        for(var i=0; i < temp.Count; ++ i)
        {
            PathNode node = temp[i];
            node.Index = i;
            nodes.Add(i, node);
        }
    }

    void GenerateCurve()
    {
        Queue<FrameDate> frames;
        List<Transform> wayPoints = new List<Transform>();
        List<PathNode> pathnodes = new List<PathNode>(nodes.Values);

        pathnodes.Sort((a, b) =>
        {
            return a.Index.CompareTo(b.Index);
        });

        int count = pathnodes.Count;
        for(int i=0; i< count; ++i)
        {
            if (i == 0)
            {
                psystem.transform.position = pathnodes[i].Node.position;
                psystem.transform.eulerAngles = Vector3.zero;
            }
            if (pathnodes[i].Node)
            {
                wayPoints.Add(pathnodes[i].Node);
            }
        }
        //至少要两个点
        if(wayPoints.Count > 1)
        {
            float distance = ParticleUtils.CalculateDirection(wayPoints, out frames);
            AnimationCurve curve_X;
            AnimationCurve curve_Y;
            AnimationCurve curve_Z;
            float lifeTime = distance / psystem.startLifetime;
            ParticleUtils.MakeCurve(frames, distance, lifeTime, out curve_X, out curve_Y, out curve_Z);
            var vel = psystem.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.Local;
            vel.x = new ParticleSystem.MinMaxCurve(lifeTime, curve_X);
            vel.y = new ParticleSystem.MinMaxCurve(lifeTime, curve_Y);
            vel.z = new ParticleSystem.MinMaxCurve(lifeTime, curve_Z);
        }

    }
}

#endif