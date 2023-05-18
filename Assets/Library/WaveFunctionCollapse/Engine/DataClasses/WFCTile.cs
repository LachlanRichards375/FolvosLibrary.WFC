using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Xml;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "WFC/Tile")]
	public class WFCTile : ScriptableObject
	{
		public string Name;
		public int TileWeight;
		public TileData TileData;
		[SerializeReference] public WFCRule[] Rules = new WFCRule[1];


		public void RuleSetup(IWFCManager manager, IWFCCell cell)
		{
			foreach (WFCRule rule in Rules)
			{
				rule.RuleInitialize(manager, cell);
			}
		}

		public static bool operator ==(WFCTile tile1, WFCTile tile2)
		{

			if (tile1 == null || tile2 == null)
			{
				return false;
			}
			return tile1.Name == tile2.Name;
		}

		public static bool operator !=(WFCTile tile1, WFCTile tile2)
		{
			return !tile1 == tile2;
		}

		public override bool Equals(object other)
		{
			if (other is WFCTile)
			{
				WFCTile otherTile = (WFCTile)other;
				return otherTile.Name == this.Name;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	[System.Serializable]
	public partial class TileData
	{
		[SerializeField] public Sprite Sprite;
		//Include other types if needed
	}
}