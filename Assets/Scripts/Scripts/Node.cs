using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtensibleNodeEditor
{

	public struct NodeData
	{
		public NodeData(Vector2 position, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle addPointStyle, Action<ConnectionPoint> OnClickAddPoint, Action<Node> OnClickRemoveNode)
		{
			this.position = position;
			this.nodeStyle = nodeStyle;
			this.selectedStyle = selectedStyle;
			this.addPointStyle = addPointStyle;
			this.OnClickAddPoint = OnClickAddPoint;
			this.OnClickRemoveNode = OnClickRemoveNode;
		}
		public Vector2 position;
		public GUIStyle nodeStyle;
		public GUIStyle selectedStyle;
		public GUIStyle addPointStyle;
		public Action<ConnectionPoint> OnClickAddPoint;
		public Action<Node> OnClickRemoveNode;
	}

	public class Node
	{
		public Rect rect;
		public string title = "New Node";
		public bool isDragged;
		public bool isSelected;

		public GUIStyle style;

		public GUIStyle defaultNodeStyle;
		public GUIStyle selectedNodeStyle;

		public Action<Node> OnRemoveNode;

		List<NodeElement> elements;

		public static float ElementPadding = 5f;
		public static float NodeEdge = 10f;

		public Node(NodeData data)
		{
			CreateHeader();

			rect = new Rect(data.position, GetElementsSize());

			PositionElements();

			AddElement(new SpriteNodeElement(this));

			style = data.nodeStyle;
			defaultNodeStyle = data.nodeStyle;
			selectedNodeStyle = data.selectedStyle;
			OnRemoveNode = data.OnClickRemoveNode;
		}

		public void CreateHeader()
		{
			elements = new List<NodeElement>();
			elements.Add(new EditableLabelNodeElement(this, "New Node"));
			elements.Add(new HorizontalRuleNode(this));
		}

		public void PositionElements()
		{
			float widthPos = rect.xMin;
			float heightPos = rect.yMin + NodeEdge;
			foreach (NodeElement e in elements)
			{
				e.rect = new Rect(widthPos + NodeEdge, heightPos, rect.width - NodeEdge * 2f, e.GetHeight());
				heightPos += e.GetHeight() + ElementPadding;
			}
		}

		public Vector2 GetElementsSize()
		{
			Vector2 returner = new Vector2(0, 0);
			float height = 50;
			float width = 200;
			if (elements != null)
			{
				foreach (NodeElement e in elements)
				{
					if (width < e.rect.width)
					{
						width = e.rect.width;
					}
					height += e.rect.height;
				}
			}

			returner.x = width;
			returner.y = height;

			return returner;
		}

		public void Drag(Vector2 delta)
		{
			rect.position += delta;
		}

		public void Draw()
		{
			PositionElements();
			rect.size = GetElementsSize();
			GUI.Box(rect, "", style);

			foreach (NodeElement e in elements)
			{
				e.Draw();
			}
		}

		public List<ConnectionPoint> GetConnectionPoints()
		{
			List<ConnectionPoint> returner = new List<ConnectionPoint>();

			foreach (NodeElement e in elements)
			{
				foreach (ConnectionPoint connectionPoint in e.GetConnectionPoints())
				{
					returner.Add(connectionPoint);
				}
			}

			return returner;
		}

		public bool ProcessEvents(Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
					{
						if (rect.Contains(e.mousePosition))
						{
							isDragged = true;
							GUI.changed = true;
							isSelected = true;
							style = selectedNodeStyle;
						}
						else
						{
							GUI.changed = true;
							isSelected = false;
							style = defaultNodeStyle;
						}
					}

					if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
					{
						ProcessContextMenu();
						e.Use();
					}

					break;

				case EventType.MouseUp:
					isDragged = false;
					break;

				case EventType.MouseDrag:
					if (e.button == 0 && isDragged)
					{
						Drag(e.delta);
						e.Use();
						return true;
					}

					break;
			}

			return false;
		}

		void ProcessContextMenu()
		{
			GenericMenu genericMenu = new GenericMenu();
			genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
			genericMenu.ShowAsContext();
		}

		void OnClickRemoveNode()
		{
			if (OnRemoveNode != null)
			{
				OnRemoveNode(this);
			}
		}

		public void AddElement(NodeElement e)
		{
			elements.Add(e);
		}
	}
}