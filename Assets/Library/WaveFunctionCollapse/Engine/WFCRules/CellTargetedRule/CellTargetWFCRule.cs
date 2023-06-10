using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class CellTargetWFCRule : WFCRule
	{
		[SerializeField] public IWFCCell targetCell;

		protected CellTargetWFCRule(WFCRule rule) : base(rule)
		{
		}
	}
}