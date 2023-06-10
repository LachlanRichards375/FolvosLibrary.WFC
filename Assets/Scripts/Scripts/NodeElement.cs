using UnityEngine;

namespace ExtensibleNodeEditor
{
	public abstract class NodeElement
	{
		public Rect rect;
		public Node parent;
		ConnectionPoint[] connectionPoints = new ConnectionPoint[0];

		public NodeElement(Node parent)
		{
			this.parent = parent;
		}

		public abstract void Draw();

		public abstract float GetHeight();

		public ConnectionPoint[] GetConnectionPoints()
		{
			return connectionPoints;
		}
	}
}