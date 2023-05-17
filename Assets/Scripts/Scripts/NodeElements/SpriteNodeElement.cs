using UnityEditor;
using UnityEngine;

namespace ExtensibleNodeEditor
{
	public class SpriteNodeElement : NodeElement
	{
		Texture2D sprite;
		public SpriteNodeElement(Node parent) : base(parent)
		{
		}

		public SpriteNodeElement(Node parent, Texture2D toDraw) : base(parent)
		{
			sprite = toDraw;
		}

		public override void Draw()
		{
			GUI.Box(rect, sprite);
		}

		public override float GetHeight()
		{
			return sprite.height;
		}
	}
}