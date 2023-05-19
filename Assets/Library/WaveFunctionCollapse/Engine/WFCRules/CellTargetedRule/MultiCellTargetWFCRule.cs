using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		[SerializeField] public List<IWFCCell> targetCells;

		public void TargetCellUpdated(WFCCellUpdate cellUpdate)
		{
			Test();
		}
	}
}