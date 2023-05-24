using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
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
			// Debug.Log("Targeted cell was updated, position: " + ((WFCCell_2D)cellUpdate.UpdatedCell).Position);
			bool t = Test();
			if (t == false)
			{
				InvokeOnRuleFail();
			}
		}
	}
}