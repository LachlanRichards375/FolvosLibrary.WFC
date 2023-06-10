using FolvosLibrary.WFC;
using System;
using System.Linq;
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

	public override bool Test()
	{
		// Debug.Log("Running Test(), targetCells: " + targetCells);
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

			if (manager.HasCollapsed(new IWFCPosition(targetCell.GetPosition())))
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
		if (cellUpdate == null)
		{
			return Test();
		}

		WFCCellUpdate update = cellUpdate.Value;

		switch (update.UpdateType)
		{
			case (CellUpdateType.Collapsed):
				return update.UpdatedCell.CollapsedTile == goal;

			case (CellUpdateType.DomainUpdate):
				bool result = false;
				//If our target's domain contains our goal

				bool[] PassTest = new bool[update.DomainChanges.Count];
				int i = 0;
				foreach (DomainChange domainChange in update.DomainChanges)
				{
					if (domainChange.UpdatedTile == goal)
					{
						if (domainChange.DomainUpdate == DomainUpdate.RemovedFromDomain)
						{
							PassTest[i] = false;
							continue;
						}
					}

					PassTest[i] = true;

					i++;
				}
				result = PassTest.All(t => t == true);

				return result;

			default:
				Debug.LogError("Uncaught cell update type: " + update.UpdateType);
				break;
		}

		return false;
	}
}