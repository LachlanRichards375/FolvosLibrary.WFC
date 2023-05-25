using System;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class CellTargetWFCRule : WFCRule
	{
		[SerializeField] public IWFCCell targetCell;

		protected CellTargetWFCRule(WFCRule rule) : base(rule)
		{
		}

		public void TargetCellUpdated(WFCCellUpdate cellUpdate)
		{
			Test(cellUpdate);
		}
	}
}