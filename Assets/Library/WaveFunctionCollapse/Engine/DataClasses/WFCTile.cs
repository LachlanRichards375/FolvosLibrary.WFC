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
				// Debug.Log("Rule setup");
				rule.RuleInitialize(manager, cell);
			}
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