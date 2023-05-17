using UnityEngine;


namespace ExtensibleNodeEditor
{
	public class EditableLabelNodeElement : NodeElement
	{
		public string text;


		public EditableLabelNodeElement(Node parent, string text) : base(parent)
		{
			this.text = text;
		}

		public override void Draw()
		{
			text = GUI.TextField(rect, text);
		}

		public override float GetHeight()
		{
			return 20f;
		}
	}
}