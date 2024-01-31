using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEngine;

[System.Serializable]
public class MultiCellIsTarget2D : MultiCellTargetWFCRule
{

	public MultiCellIsTarget2D() : base()
	{

	}

	public MultiCellIsTarget2D(MultiCellIsTarget2D other) : base(other)
	{

	}
	public override bool Test(WFCCellUpdate? cellUpdate, WFCCell OwnerCell)
	{
		if (cellUpdate == null)
		{
			return false;
		}

		WFCCellUpdate update = cellUpdate.Value;

		switch (update.UpdateType)
		{
			case (CellUpdateType.Collapsed):
				return update.UpdatedCell.CollapsedTile.Equals(goal);

			case (CellUpdateType.DomainUpdate):
				return (update.RemovedFromDomain & goal.ID) != goal.ID;

			default:
				Debug.LogError("Uncaught cell update type: " + update.UpdateType);
				break;
		}

		return false;
	}
}