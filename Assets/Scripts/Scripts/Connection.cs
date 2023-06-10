using System;
using UnityEditor;
using UnityEngine;


namespace ExtensibleNodeEditor
{
	public class Connection
	{
		public ConnectionPoint[] points = new ConnectionPoint[2];

		public Action<Connection> OnClickRemoveConnection;

		public Connection(ConnectionPoint point1, ConnectionPoint point2, Action<Connection> OnClickRemoveConnection)
		{
			this.points[0] = point1;
			this.points[1] = point2;
			this.OnClickRemoveConnection = OnClickRemoveConnection;
		}

		public void Draw()
		{
			Handles.DrawBezier(
				points[0].rect.center,
				points[1].rect.center,
				points[0].rect.center + Vector2.left * 50f,
				points[1].rect.center - Vector2.left * 50f,
				Color.white,
				null,
				2f
			);

			if (Handles.Button((points[0].rect.center + points[1].rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
			{
				if (OnClickRemoveConnection != null)
				{
					OnClickRemoveConnection(this);
				}
			}
		}

	}
}