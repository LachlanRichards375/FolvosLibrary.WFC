using UnityEngine;


namespace ExtensibleNodeEditor
{
	public class HorizontalRuleNode : NodeElement
	{
		public HorizontalRuleNode(Node parent) : base(parent)
		{
		}

		public override void Draw()
		{
			GUI.Box(rect, "");
		}

		public override float GetHeight()
		{
			return 10f;
		}
	}
}