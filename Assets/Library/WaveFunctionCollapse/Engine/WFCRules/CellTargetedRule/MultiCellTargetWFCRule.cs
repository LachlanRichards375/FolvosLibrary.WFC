using System;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		[SerializeField] public IWFCCell[] targetCells;

		public void TargetCellUpdated(WFCCellUpdate cellUpdate)
		{
			Test();
		}
	}
}