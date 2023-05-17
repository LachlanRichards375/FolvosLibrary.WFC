using System;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class CellTargetWFCRule : WFCRule
	{
		[SerializeField] public IWFCCell targetCell;

		public void TargetCellUpdated(WFCCellUpdate cellUpdate)
		{
			Test();
		}
	}
}