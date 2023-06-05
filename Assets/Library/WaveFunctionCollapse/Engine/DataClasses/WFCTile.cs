using System.Linq;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/Tile"), System.Serializable]
	public class WFCTile : ScriptableObject
	{
		public string Name;
		[Min(1)] public int TileWeight = 1;
		public TileData TileData;
		[SerializeReference] public WFCRule[] Rules = new WFCRule[1];

		public void Copy(WFCTile other)
		{
			Name = other.Name;
			TileWeight = other.TileWeight;
			TileData = other.TileData;
			Rules = other.Rules;
		}

		public void RuleSetup(IWFCManager manager, IWFCCell cell)
		{
			for (int i = 0; i < Rules.Length; i++)
			{
				//Do a method overload to return new objects?
				//This is needed so we aren't using the same Rule Objects as other tiles
				Rules[i] = (WFCRule)System.Activator.CreateInstance(Rules[i].GetType(), Rules[i]);
				Rules[i].RuleInitialize(manager, cell.GetPosition().AsVector2Int());
				Rules[i].OnRuleActivated += cell.DomainCheck;
			}
		}

		public bool PassesRules(WFCCellUpdate update, IWFCCell caller)
		{
			int index = 0;
			bool returner = true;
			for (int i = 0; i < Rules.Length; i++)
			{
				returner = Rules[index].Test(update, caller);
				if (returner == false)
				{
					Debug.Log($"Testing {Rules[index].GetType()}(index: {index}) from cell {caller.GetPositionString()} targeting {update.UpdatedCell.GetPositionString()} failed"
					+ $" > {Rules[index].GetTargetCells()}");
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

	}

	[System.Serializable]
	public partial class TileData
	{
		[SerializeField] public Sprite Sprite;
		//Include other types if needed
	}
}