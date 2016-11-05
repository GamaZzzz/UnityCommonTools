using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class TouchPad : MonoBehaviour, IDragHandler
{
    private Vector2 lastValue = Vector2.zero;

    void Start()
    { 

    }

    void On_Value_Changed(Vector2 arg)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("PointerEventData:" + eventData.delta);
    }
}

