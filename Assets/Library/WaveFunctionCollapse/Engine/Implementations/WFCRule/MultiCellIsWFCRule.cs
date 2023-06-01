using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class MultiCellIsTarget2D : MultiCellTargetWFCRule
{
	[SerializeField] public WFCTile goal;
	public CellDirection.Direction direction;

	public MultiCellIsTarget2D() : base()
	{

	}

	public MultiCellIsTarget2D(MultiCellIsTarget2D multiCellIsTarget2D) : base(multiCellIsTarget2D)
	{
		goal = multiCellIsTarget2D.goal;
		direction = multiCellIsTarget2D.direction;
	}

	public override void DrawRuleProperties()
	{
		goal = (WFCTile)EditorGUILayout.ObjectField("Goal Tile: ", goal, typeof(WFCTile), true);
		direction = (CellDirection.Direction)EditorGUILayout.EnumFlagsField("Directions: ", direction);
	}

	public override bool Test()
	{
		// Debug.Log("Running Test(), targetCells: " + targetCells);
		if (targetCells == null)
		{
			return false;
		}

		bool[] PassTest = new bool[targetCells.Count];
		int i = 0;
		// foreach (IWFCCell ICell in targetCells)
		foreach (Vector2Int pos in targetCells)
		{

			WFCCell_2D targetCell = (WFCCell_2D)GetTargetCell(pos);

			Debug.Log($"Target cell({targetCell.Position},{targetCell.GetHashCode()}) is collapsed: {(manager as WFCManager_2D).HasCollapsed(targetCell.Position)}, == null? {targetCell.CollapsedTile == null}, is null? {targetCell.CollapsedTile is null}, is same as manager? || EQUAL TO MANAGER VERSION? {(manager as WFCManager_2D).GetCell(targetCell.Position).Equals(targetCell)}");

			// if (targetCell.CollapsedTile != null)
			if ((manager as WFCManager_2D).HasCollapsed(targetCell.Position))
			{
				Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} didPass? {targetCell.CollapsedTile == goal}\n" +
				$"Target cell is collapsed, {targetCell.CollapsedTile.Name} == {goal.Name}? {targetCell.CollapsedTile == goal}");
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
			Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} didPass? {PassTest[i]}\n" + toPrint);
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

	public override bool Test(WFCCellUpdate? cellUpdate)
	{
		if (cellUpdate == null)
		{
			return Test();
		}

		WFCCellUpdate update = cellUpdate.Value;
		WFCCell_2D targetCell = (WFCCell_2D)update.UpdatedCell;

		Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position}. Rule Hash: {GetHashCode()}, OwnerCell.HashCode: {OwnerCell.GetHashCode()}");

		switch (update.UpdateType)
		{
			case (CellUpdateType.Collapsed):
				// Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} had goal {goal.Name} didPass? {targetCell.CollapsedTile == goal}\n" +
				// 			$"Target cell is collapsed, {targetCell.CollapsedTile.Name} == {goal.Name}? {targetCell.CollapsedTile == goal}");
				return targetCell.CollapsedTile == goal;

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

	public override void RuleInitialize(IWFCManager manager, Vector2Int CellPos)
	{
		WFCManager_2D m = manager as WFCManager_2D;
		WFCCell_2D cell = m.GetCell(CellPos) as WFCCell_2D;

		this.manager = manager;

		OwnerCell = cell;
		// Debug.Log($"Rule at {cell.Position},{GetHashCode()}, OwnerCell.HashCode: {OwnerCell.GetHashCode()}");

		//For each possible direction
		foreach (CellDirection.Direction currentDirection in (CellDirection.Direction[])Enum.GetValues(typeof(CellDirection.Direction)))
		{

			//If there is a cell in this direction add it to targetCells
			Vector2Int direction = cell.Position + CellDirection.CellDirectionToVector2Int(currentDirection);
			IWFCCell targetCell = m.GetCell(direction);
			if (targetCell != null)
			{

				if (targetCells == null)
					targetCells = new System.Collections.Generic.List<Vector2Int>();

				targetCells.Add(direction);

				// When the target cell is updated cause our cell to do a domain check
				targetCell.OnCellUpdate += InvokeRuleActivated;
			}
		}
	}

	// public override void RuleInitialize(IWFCManager manager, IWFCCell Cell)
	// {
	// 	WFCManager_2D m = manager as WFCManager_2D;
	// 	WFCCell_2D cell = Cell as WFCCell_2D;

	// 	this.manager = manager;

	// 	OwnerCell = cell;
	// 	Debug.Log($"Rule at {cell.Position},{GetHashCode()}, OwnerCell.HashCode: {OwnerCell.GetHashCode()}");

	// 	//For each possible direction
	// 	foreach (CellDirection.Direction currentDirection in (CellDirection.Direction[])Enum.GetValues(typeof(CellDirection.Direction)))
	// 	{

	// 		//If there is a cell in this direction add it to targetCells
	// 		Vector2Int direction = cell.Position + CellDirection.CellDirectionToVector2Int(currentDirection);
	// 		IWFCCell targetCell = m.GetCell(direction);
	// 		if (targetCell != null)
	// 		{

	// 			Debug.Log($"Target cell at {direction} from {cell.Position} is not null");

	// 			if (targetCells == null)
	// 				targetCells = new System.Collections.Generic.List<Vector2Int>();

	// 			targetCells.Add(direction);

	// 			// When the target cell is updated cause our cell to do a domain check
	// 			targetCell.OnCellUpdate += InvokeRuleActivated;
	// 		}
	// 	}
	// }

	IWFCCell GetTargetCell(Vector2Int pos)
	{
		return ((WFCManager_2D)manager).GetCell(pos);
	}
}