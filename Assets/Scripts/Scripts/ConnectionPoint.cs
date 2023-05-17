
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace ExtensibleNodeEditor {

public class ConnectionPoint
{
    public Rect rect;

    public Vector2 localPosition;
 
    public Node node;
 
    public GUIStyle style;
 
    public Action<ConnectionPoint> OnClickConnectionPoint;
    
    public ConnectionPoint(Node node, Vector2 localPosition, GUIStyle style, Action<ConnectionPoint> OnClickConnectionPoint)
    {
        this.node = node;
        this.localPosition = localPosition;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        rect = new Rect(0, 0, 10f, 20f);
    }
 
    public void Draw()
    {
        rect.x = localPosition.x;
        rect.y = localPosition.y;
        
        if (GUI.Button(rect, "", style))
        {
            if (OnClickConnectionPoint != null)
            {
                OnClickConnectionPoint(this);
            }
        }
    }
}
}