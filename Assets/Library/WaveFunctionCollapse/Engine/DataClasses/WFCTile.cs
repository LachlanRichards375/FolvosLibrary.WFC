using System.Linq;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/Tile"), System.Serializable]
	public class WFCTile : ScriptableObject
	{
		public string Name;
		public int TileWeight;
		public TileData TileData;
		[SerializeReference] public WFCRule[] Rules = new WFCRule[1];

		IWFCManager manager;

		public void RuleSetup(IWFCManager manager, IWFCCell cell)
		{
			this.manager = manager;
			for (int i = 0; i < Rules.Length; i++)
			{
				//Do a method overload to return new objects?
				//This is needed so we aren't using the same Rule Objects as other tiles
				WFCRule rule = (WFCRule)System.Activator.CreateInstance(Rules[i].GetType(), Rules[i]);
				rule.RuleInitialize(manager, cell);
				rule.OnRuleActivated += (WFCCellUpdate update) =>
				{
					bool passesTest = PassesRules(update);
					if (!passesTest)
					{
						cell.DomainCheck(update);
					}
				};
				Rules[i] = rule;
			}
		}

		public bool PassesRules(WFCCellUpdate update)
		{
			bool returner = false;
			for (int i = 0; i < Rules.Length; i++)
			{
				returner = Rules[i].Test(update);
				if (returner == true)
				{
					break;
				}
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

		public bool PassesRules()
		{
			bool returner = true;
			bool[] PassesTest = new bool[Rules.Length];
			for (int i = 0; i < Rules.Length; i++)
			{
				returner = Rules[i].Test();
				if (returner == false)
				{
					break;
				}
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