using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Draw(Vector3 from, Vector3 to, Color color, float width = 0.001f)
    {
        lineRenderer.SetColors(color, color);
        lineRenderer.SetPositions(new Vector3[] { from, to });
        lineRenderer.SetWidth(width, width);
    }

#if UNITY_EDITOR
    void Reset()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }
#endif
}