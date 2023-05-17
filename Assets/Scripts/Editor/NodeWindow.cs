using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ExtensibleNodeEditor
{

	public class NodeWindow : EditorWindow
	{

		private List<Node> nodes;
		private List<Connection> connections;

		public GUIStyle nodeStyle;
		public GUIStyle selectedNodeStyle;
		public GUIStyle addPointStyle;

		private ConnectionPoint selectedAddPoint;

		private Vector2 offset;
		private Vector2 drag;

		[MenuItem("Window/Node Based Editor")]
		private static void OpenWindow()
		{
			NodeWindow window = GetWindow<NodeWindow>();
			window.titleContent = new GUIContent("Node Based Editor");
		}

		public void OpenCustomWindow()
		{
			NodeWindow.OpenWindow();
		}

		private void OnEnable()
		{
			nodeStyle = new GUIStyle();
			nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
			nodeStyle.border = new RectOffset(12, 12, 12, 12);
			nodeStyle.alignment = TextAnchor.UpperCenter;

			selectedNodeStyle = new GUIStyle();
			selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
			selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

			addPointStyle = new GUIStyle();
			addPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn top.png") as Texture2D;
			addPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn top on.png") as Texture2D;
			addPointStyle.border = new RectOffset(4, 4, 12, 12);
		}

		private void OnGUI()
		{
			DrawGrid(20, 0.2f, Color.gray); //Major Lines
			DrawGrid(100, 0.4f, Color.gray); //Minor Lines

			DrawNodes();
			DrawConnections();

			DrawConnectionLine(Event.current);

			ProcessNodeEvents(Event.current);
			ProcessEvents(Event.current);

			if (GUI.changed) Repaint();
		}

		private void DrawGrid(float linesToDraw, float gridOpacity, Color gridColor)
		{
			int widthDivs = Mathf.CeilToInt(position.width / linesToDraw);
			int heightDivs = Mathf.CeilToInt(position.height / linesToDraw);

			Handles.BeginGUI();
			Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

			offset += drag * 0.5f;
			Vector3 newOffset = new Vector3(offset.x % linesToDraw, offset.y % linesToDraw, 0);

			for (int i = -1; i <= widthDivs; i++)
			{
				Handles.DrawLine(new Vector3(linesToDraw * i, -position.height, 0) + newOffset, new Vector3(linesToDraw * i, position.height, 0f) + newOffset);
			}

			for (int j = -1; j <= heightDivs; j++)
			{
				Handles.DrawLine(new Vector3(-position.width, linesToDraw * j, 0) + newOffset, new Vector3(position.width, linesToDraw * j, 0f) + newOffset);
			}

			Handles.color = Color.white;
			Handles.EndGUI();
		}

		private void DrawNodes()
		{
			if (nodes != null)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					if (nodes[i] == null)
					{
						nodes.RemoveAt(i);
						continue;
					}
					nodes[i].Draw();
				}
			}
		}

		private void DrawConnections()
		{
			if (connections != null)
			{
				for (int i = 0; i < connections.Count; i++)
				{
					if (connections[i] == null)
					{
						connections.RemoveAt(i);
						continue;
					}
					connections[i].Draw();
				}
			}
		}

		private void ProcessEvents(Event e)
		{
			drag = Vector2.zero;

			switch (e.type)
			{
				case EventType.MouseDown:
					if (e.button == 0)
					{
						ClearConnectionSelection();
					}

					if (e.button == 1)
					{
						ProcessContextMenu(e.mousePosition);
					}
					break;

				case EventType.MouseDrag:
					if (e.button == 0)
					{
						OnDrag(e.delta);
					}
					break;
			}
		}

		private void ProcessNodeEvents(Event e)
		{
			if (nodes != null)
			{
				for (int i = nodes.Count - 1; i >= 0; i--)
				{
					bool guiChanged = nodes[i].ProcessEvents(e);

					if (guiChanged)
					{
						GUI.changed = true;
					}
				}
			}
		}

		private void DrawConnectionLine(Event e)
		{
			if (selectedAddPoint != null)
			{
				Handles.DrawBezier(
					selectedAddPoint.rect.center,
					e.mousePosition,
					selectedAddPoint.rect.center + Vector2.left * 50f,
					e.mousePosition - Vector2.left * 50f,
					Color.white,
					null,
					2f
				);

				GUI.changed = true;
			}
		}

		private void ProcessContextMenu(Vector2 mousePosition)
		{
			GenericMenu genericMenu = new GenericMenu();
			NodeData nodeData = new NodeData(mousePosition, nodeStyle, selectedNodeStyle, addPointStyle, OnClickAddPoint, OnClickRemoveNode);

			genericMenu.AddItem(new GUIContent("Nodes/Blank Node"), false, () => OnClickAddNode(mousePosition, new Node(nodeData)));
			genericMenu.AddItem(new GUIContent("Nodes/Example Node"), false, () => OnClickAddNode(mousePosition, new ExampleNode(nodeData)));
			genericMenu.ShowAsContext();
		}

		private void OnDrag(Vector2 delta)
		{
			drag = delta;

			if (nodes != null)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					nodes[i].Drag(delta);
				}
			}

			GUI.changed = true;
		}

		private void OnClickAddNode(Vector2 mousePosition, Node toAdd)
		{
			if (nodes == null)
			{
				nodes = new List<Node>();
			}

		}

		private void OnClickAddPoint(ConnectionPoint inPoint)
		{
			if (selectedAddPoint != null)
			{
				if (inPoint.node != selectedAddPoint.node)
				{
					CreateConnection(inPoint);
					ClearConnectionSelection();
				}
				else
				{
					ClearConnectionSelection();
				}
			}
		}

		private void OnClickRemoveNode(Node node)
		{
			if (connections != null)
			{
				List<Connection> connectionsToRemove = new List<Connection>();

				for (int i = 0; i < connections.Count; i++)
				{
					if (node.GetConnectionPoints().Contains(connections[i].points[0]) || node.GetConnectionPoints().Contains(connections[i].points[1]))
					{
						connectionsToRemove.Add(connections[i]);
					}
				}

				for (int i = 0; i < connectionsToRemove.Count; i++)
				{
					connections.Remove(connectionsToRemove[i]);
				}

				connectionsToRemove = null;
			}

			nodes.Remove(node);
		}

		private void OnClickRemoveConnection(Connection connection)
		{
			connections.Remove(connection);
		}

		private void CreateConnection(ConnectionPoint other)
		{
			if (connections == null)
			{
				connections = new List<Connection>();
			}

			connections.Add(new Connection(selectedAddPoint, other, OnClickRemoveConnection));
		}

		private void ClearConnectionSelection()
		{
			selectedAddPoint = null;
		}
	}


}