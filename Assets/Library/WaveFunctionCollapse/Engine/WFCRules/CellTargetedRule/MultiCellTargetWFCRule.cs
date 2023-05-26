using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		[SerializeField] public List<IWFCCell> targetCells;

		protected MultiCellTargetWFCRule(): base(){
			
		}

		protected MultiCellTargetWFCRule(WFCRule rule) : base(rule)
		{
		}

		public void TargetCellUpdated(WFCCellUpdate cellUpdate)
		{
			bool t = Test(cellUpdate);
			if (t == false)
			{
				InvokeOnRuleFail();
			}
		}
	}
}