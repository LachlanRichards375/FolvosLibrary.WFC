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

	public override bool Test()
	{
		Debug.Log("Test no params");
		if (targetCells == null)
		{
			return false;
		}

		bool[] PassTest = new bool[targetCells.Length];
		int i = 0;
		// foreach (IWFCCell ICell in targetCells)
		foreach (Vector2Int pos in targetCells)
		{

			IWFCCell targetCell = GetTargetCell(pos);

			// if (targetCell.CollapsedTile != null)
			if ((manager as WFCManager_2D).HasCollapsed(targetCell.GetPosition()))
			{
				PassTest[i] = targetCell.CollapsedTile == goal;
				i++;
				continue;
			}
			Debug.Log("Cell was not collapsed");

			PassTest[i] = false;
			string toPrint = "";
			//If our target's domain contains our goal
			for (int target = 0; target < targetCell.Domain.Count; target++)
			{
				if (targetCell.Domain != null && targetCell.Domain[target] != null)
				{
					toPrint += $"> TargetCell.Domain {targetCell.Domain[target].Name} == {goal.Name}? {targetCell.Domain[target] == goal}\n";
				}
				if (targetCell.Domain[target] == goal)
				{
					PassTest[i] = true;
					break;
				}
			}
			i++;
		}

		String s = "> PassTest:[";
		foreach (bool b in PassTest)
		{
			s += b.ToString() + ", ";
		}
		Debug.Log(s + "]");
		//if all are true, return true
		return PassTest.All(b => b == true);
	}

	public override bool Test(WFCCellUpdate? cellUpdate, IWFCCell OwnerCell)
	{
		if (cellUpdate is null)
		{
			return Test();
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
	bool CheckCellUpdateAffectsThisRule(WFCCellUpdate update, IWFCCell ownerCell)
	{
		IWFCPosition updatePos = update.UpdatedCell.GetPosition();
		IWFCPosition ownerPos = ownerCell.GetPosition();

		return direction.HasFlag(CellDirection.Vector2IntToCellDirection(updatePos.AsVector2Int() - ownerPos.AsVector2Int()));
	}

	bool CellCollapsed(IWFCCell collapsedCell, IWFCCell owner)
	{
		Debug.Log("Cell Collapsed");
		return !(collapsedCell.CollapsedTile.Equals(goal));
	}

	bool DomainUpdated(WFCCellUpdate update, IWFCCell owner)
	{
		bool result = false;
		//If our target's domain contains our goal



		// bool[] PassTest = new bool[update.DomainChanges.Count];
		// int i = 0;
		foreach (DomainChange domainChange in update.DomainChanges)
		{
			if (domainChange.UpdatedTile.Name == goal.Name)
			{
				if (domainChange.DomainUpdate == DomainUpdate.AddedToDomain)
				{
					Debug.Log($"Domain updated, {goal} at {owner.GetPosition()} is NOT allowed.");
					return false;
				}
				// else if (domainChange.DomainUpdate == DomainUpdate.RemovedFromDomain)
				// {
				// 	PassTest[i] = true;
				// }
			}

			// PassTest[i] = true;

			// i++;
		}
		// result = PassTest.All(t => t == true);

		Debug.Log($"Domain updated, {goal} at {owner.GetPosition()} is allowed.");

		// return result;
		return true;
	}

	public Vector2Int[] GetTargetCellsArray()
	{
		return targetCells;
	}
}