using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEditor;
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

	bool CellCollapsed(IWFCCell collapsedCell, IWFCCell owner)
	{
		// string s = $"Targeted cells for {goal.Name} at {owner.GetPositionString()} contains {collapsedCell.GetPosition().AsVector2Int()}? ";
		// foreach (Vector2Int v in targetCells)
		// {
		// 	s += $"{v}, ";
		// }
		// Debug.Log(s);

		// if (targetCells.Contains(collapsedCell.GetPosition().AsVector2Int()))
		// {
		// 	Debug.Log($"targeted cell at position {collapsedCell.GetPositionString()} collapsed. CollapsedTile: {collapsedCell.CollapsedTile.Name} is not {goal.Name}? {collapsedCell.CollapsedTile != goal}");
		Debug.Log($"targetCells contains collapsedCell {collapsedCell.GetPosition().AsVector2Int()}? {targetCells.Contains(collapsedCell.GetPosition().AsVector2Int())}");
		return collapsedCell.CollapsedTile != goal;
		// }
		// else
		// {
		// 	Debug.Log($"NON targeted cell at position {collapsedCell.GetPositionString()} collapsed. we don't care");
		// 	return true;
		// }
	}

	bool DomainUpdated(WFCCellUpdate update, IWFCCell owner)
	{
		// Debug.Log("Domain has been updated");
		bool result = false;
		//If our target's domain contains our goal

		bool[] PassTest = new bool[update.DomainChanges.Count];
		int i = 0;
		foreach (DomainChange domainChange in update.DomainChanges)
		{

			if (domainChange.UpdatedTile == goal)
			{
				if (domainChange.DomainUpdate == DomainUpdate.AddedToDomain)
				{
					PassTest[i] = false;
				}
				else if (domainChange.DomainUpdate == DomainUpdate.RemovedFromDomain)
				{
					PassTest[i] = true;
				}
			}

			PassTest[i] = true;

			i++;
		}
		result = PassTest.All(t => t == true);

		return result;
	}

	public Vector2Int[] GetTargetCellsArray(){
		return targetCells;
	}
}