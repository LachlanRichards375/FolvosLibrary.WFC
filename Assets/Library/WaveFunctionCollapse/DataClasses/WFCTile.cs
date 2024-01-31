using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/Tile/Tile", order = 2), System.Serializable]
	public class WFCTile : ScriptableObject
	{
		public string Name { get => TileData.Name; set => TileData.Name = value; }
		public int TileWeight { get => TileData.TileWeight; set => TileData.TileWeight = value; }
		public ulong ID { get => TileData.BitMaskID; set => TileData.BitMaskID = value; }
		public TileData TileData;
		[SerializeReference] public WFCRule[] Rules = new WFCRule[0];

		//Needed for Unit Tests
		public static WFCTile CreateTile(TileData data, WFCRule[] Rules)
		{
			WFCTile returner = (WFCTile)ScriptableObject.CreateInstance(typeof(WFCTile));
			returner.TileData = data;
			returner.Rules = Rules;
			return returner;
		}

		public static WFCTile CreateTile(WFCTile tile)
		{
			WFCTile returner = (WFCTile)ScriptableObject.CreateInstance(typeof(WFCTile));
			returner.TileData = tile.TileData;
			returner.Rules = tile.Rules;
			return returner;
		}

		public void Copy(WFCTile other)
		{
			TileData = other.TileData;
			Rules = other.Rules;
		}

		public void RemoveRule(WFCRule toRemove)
		{
			for (int i = 0; i < Rules.Length; i++)
			{
				if (Rules[i] == toRemove)
				{
					Rules = RemoveAt(i);
				}
			}
		}

		WFCRule[] RemoveAt(int index)
		{
			if (index < 0 || index >= Rules.Length)
			{
				return null;
			}

			WFCRule[] returner = new WFCRule[Rules.Length - 1];

			int RulesCount = 0, returnerCount = 0;
			while (RulesCount < Rules.Length)
			{

				if (RulesCount != index)
				{
					returner[returnerCount] = Rules[RulesCount];
					returnerCount++;
				}

				RulesCount++;
			}

			return returner;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override bool Equals(object other)
		{
			if (other is null || !(other is WFCTile))
			{
				return false;
			}
			WFCTile otherTile = other as WFCTile;

			return otherTile.ID == this.ID;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	[System.Serializable]
	public partial class TileData
	{
		public TileData()
		{

		}

		public TileData(string Name, int TileWeight, ulong ID)
		{
			this.Name = Name;
			this.TileWeight = TileWeight;
			BitMaskID = ID;
		}

		[SerializeField]
		public ulong BitMaskID;
		private static ulong ClaimedBitMaskID;

		public string Name;
		[Min(1)] public int TileWeight = 1;
		[SerializeField] public Sprite Sprite;
	}


}