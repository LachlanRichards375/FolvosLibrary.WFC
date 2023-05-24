using System.Linq;
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
			for (int i = 0; i < Rules.Length; i++)
			{
				//Do a method overload to return new objects?
				WFCRule rule = (WFCRule)System.Activator.CreateInstance(Rules[i].GetType(), Rules[i]);
				rule.RuleInitialize(manager, cell);
				rule.OnRuleFail += RemoveRule;
				Rules[i] = rule;
			}
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
			bool[] PassesTest = new bool[Rules.Length];
			for (int i = 0; i < Rules.Length; i++)
			{
				PassesTest[i] = Rules[i].Test();
			}

			return PassesTest.Any(t => t == true);
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