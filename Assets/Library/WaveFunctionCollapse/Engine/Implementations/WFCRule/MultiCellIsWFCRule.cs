using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

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
		Debug.Log("Running Test(), targetCells: " + targetCells);
		if (targetCells == null)
		{
			return false;
		}

		bool[] PassTest = new bool[targetCells.Count];
		int i = 0;
		foreach (IWFCCell ICell in targetCells)
		{
			WFCCell_2D targetCell = (WFCCell_2D)ICell;

			if (targetCell.CollapsedTile != null)
			{
				Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} didPass? {targetCell.CollapsedTile == goal}\n" +
							$"Target cell is collapsed, {targetCell.CollapsedTile.Name} == {goal.Name}? {targetCell.CollapsedTile == goal}");
				PassTest[i] = targetCell.CollapsedTile == goal;
				continue;
			}

			PassTest[i] = false;
			string toPrint = "";
			//If our target's domain contains our goal
			for (int target = 0; target < targetCell.Domain.Count; target++)
			{
				toPrint += $"> TargetCell.Domain {targetCell.Domain[target].Name} == {goal.Name}? {targetCell.Domain[target] == goal}\n";
				if (targetCell.Domain[target] == goal)
				{
					PassTest[i] = true;
					break;
				}
			}
			Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} didPass? {PassTest[i]}\n" + toPrint);
			i++;
		}

		//if any are false return false
		return !PassTest.Any(b => b == false);
	}

	public override bool Test(WFCCellUpdate? cellUpdate)
	{
		Debug.Log("Testing Rule, has cell Update: " + (cellUpdate is null));
		if (cellUpdate == null)
		{
			Debug.Log("Rule.Test was not provided a cell update");
			return Test();
		}

		WFCCellUpdate update = cellUpdate.Value;
		WFCCell_2D targetCell = (WFCCell_2D)update.UpdatedCell;

		switch (update.UpdateType)
		{
			case (CellUpdateType.Collapsed):
				Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} didPass? {targetCell.CollapsedTile == goal}\n" +
							$"Target cell is collapsed, {targetCell.CollapsedTile.Name} == {goal.Name}? {targetCell.CollapsedTile == goal}");
				return targetCell.CollapsedTile == goal;

			case (CellUpdateType.DomainUpdate):
				bool result = false;
				string toPrint = "";
				//If our target's domain contains our goal

				foreach (DomainChange domainChange in update.DomainChanges)
				{
					if (domainChange.UpdatedTile == goal)
					{
						if (domainChange.DomainUpdate == DomainUpdate.RemovedFromDomain)
						{
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
				Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position} didPass? {result}\n" + toPrint);

				return result;

			default:
				Debug.LogError("Uncaught cell update type: " + update.UpdateType);
				break;
		}

		return false;
	}

	public override void RuleInitialize(IWFCManager manager, IWFCCell Cell)
	{
		WFCManager_2D m = manager as WFCManager_2D;
		WFCCell_2D cell = Cell as WFCCell_2D;

		OwnerCell = cell;

		//For each possible direction
		foreach (CellDirection.Direction currentDirection in (CellDirection.Direction[])Enum.GetValues(typeof(CellDirection.Direction)))
		{

			//If there is a cell in this direction add it to targetCells
			Vector2Int direction = cell.Position + CellDirection.CellDirectionToVector2Int(currentDirection);
			IWFCCell targetCell = m.GetCell(direction);
			if (targetCell != null)
			{
				// Debug.Log($"cellPos = {cell.Position} + {currentDirection} {CellDirection.CellDirectionToVector2Int(currentDirection)} = {direction} ");
				if (targetCells == null)
					targetCells = new System.Collections.Generic.List<IWFCCell>();

				targetCells.Add(targetCell);

				// When the target cell is updated cause our cell to do a domain check
				targetCell.OnCellUpdate += TargetCellUpdated;
				// targetCell.OnCellUpdate += (WFCCellUpdate u) => cell.DomainCheck();
			}
		}
	}
}