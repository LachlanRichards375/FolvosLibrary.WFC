using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEngine;

[System.Serializable]
public class MultiCellIsNotTarget2D : MultiCellTargetWFCRule
{
	public MultiCellIsNotTarget2D() : base()
	{

	}

	public MultiCellIsNotTarget2D(MultiCellIsNotTarget2D other) : base(other)
	{

	}

	public override bool Test(WFCCellUpdate? cellUpdate, WFCCell OwnerCell)
	{
		if (cellUpdate is null)
		{
			return false;
		}

		WFCCellUpdate update = cellUpdate.Value;

		if (!CheckCellUpdateAffectsThisRule(update, OwnerCell))
		{
			return true;
		}

		switch (update.UpdateType)
		{
			case (CellUpdateType.Collapsed):
				return CellCollapsed(update.UpdatedCell, OwnerCell);

			case (CellUpdateType.DomainUpdate):
				return DomainUpdated(cellUpdate.Value, OwnerCell);

			default:
				Debug.LogError("Uncaught cell update type: " + update.UpdateType);
				break;
		}

		return false;
	}

	//Return true if this rule impacts owner cell
	bool CheckCellUpdateAffectsThisRule(WFCCellUpdate update, WFCCell ownerCell)
	{
		WFCPosition updatePos = update.UpdatedCell.GetPosition();
		WFCPosition ownerPos = ownerCell.GetPosition();

		return direction.HasFlag(CellDirection.Vector2IntToCellDirection(updatePos.AsVector2Int() - ownerPos.AsVector2Int()));
	}

	bool CellCollapsed(WFCCell collapsedCell, WFCCell owner)
	{
		return !(collapsedCell.CollapsedTile.Equals(goal));
	}

	bool DomainUpdated(WFCCellUpdate update, WFCCell owner)
	{
		//Always return true because we only care
		//That our target cell does NOT collapse to our goal
		//We don't care if the goal is in the domain
		return true;
	}

	public Vector2Int[] GetTargetCellsArray()
	{
		return targetCells;
	}
}