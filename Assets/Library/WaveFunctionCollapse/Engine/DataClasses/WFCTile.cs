using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/Tile"), System.Serializable]
	public class WFCTile : ScriptableObject
	{
		public string Name { get => TileData.Name; set => TileData.Name = value; }
		public int TileWeight { get => TileData.TileWeight; set => TileData.TileWeight = value; }
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
			Name = other.Name;
			TileWeight = other.TileWeight;
			TileData = other.TileData;
			Rules = other.Rules;
		}

		public void RuleSetup(IWFCManager manager, WFCCell cell)
		{
			WFCRule[] NewRules = new WFCRule[Rules.Length];
			for (int i = 0; i < Rules.Length; i++)
			{
				//Do a method overload to return new objects?
				//This is needed so we aren't using the same Rule Objects as other tiles
				NewRules[i] = (WFCRule)System.Activator.CreateInstance(Rules[i].GetType(), Rules[i]);
				NewRules[i].RuleInitialize(manager, cell.GetPosition().AsVector2Int());
				// NewRules[i].OnRuleActivated += (WFCCellUpdate update) => cell.DomainCheck(update);
			}
			Rules = NewRules;
		}

		public bool PassesRules(WFCCellUpdate update, WFCCell caller)
		{
			int index = 0;
			bool returner = true;
			for (int i = 0; i < Rules.Length; i++)
			{
				returner = Rules[index].Test(update, caller);
				if (returner == false)
				{
					break;
				}
				index++;
			}

			return returner;
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

			return otherTile.Name == this.Name;
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

		public TileData(string Name, int TileWeight)
		{
			this.Name = Name;
			this.TileWeight = TileWeight;
		}

		public string Name;
		[Min(1)] public int TileWeight = 1;
		[SerializeField] public Sprite Sprite;
		//Include other types if needed
	}
}