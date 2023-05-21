using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

public class MultiCellIsTarget2D : MultiCellTargetWFCRule
{
	[SerializeField] public WFCTile goal;
	public CellDirection.Direction direction;

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
		if (targetCells == null)
		{
			return false;
		}

		bool[] PassTest = new bool[targetCells.Count];
		int i = 0;
		foreach (IWFCCell ICell in targetCells)
		{

			WFCCell_2D targetCell = (WFCCell_2D)ICell;


			//IF cell has collapsed and is equal to our goal
			//return targetCell.CollapsedTile == goal
			if (targetCell.CollapsedTile != null)
			{
				// Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position}");
				// Debug.Log($"Target cell is collapsed, {targetCell.CollapsedTile.Name} == {goal.Name}? {targetCell.CollapsedTile == goal}");
				PassTest[i] = targetCell.CollapsedTile == goal;
				continue;
			}

			// Debug.Log("TargetCell is not collapsed, searching target domain");
			PassTest[i] = false;
			//If our target's domain contains our goal
			for (int target = 0; target < targetCell.Domain.Length; target++)
			{
				// Debug.Log($"TargetCell.Domain {targetCell.Domain[target].Name} == {goal.Name}? {targetCell.Domain[target] == goal}");
				if (targetCell.Domain[target] == goal)
				{
					PassTest[i] = true;
					break; //break out of for target loop, continue to next target cell
				}
			}
			// Debug.Log($"Cell {((WFCCell_2D)OwnerCell).Position} targeting {targetCell.Position}");
			// Debug.Log($"PassTest == {PassTest[i]}");

			i++;
		}

		//if any are false return false
		return !PassTest.Any(b => b == false);
	}

	public override void RuleInitialize(IWFCManager manager, IWFCCell Cell)
	{
		WFCManager_2D m = manager as WFCManager_2D;
		WFCCell_2D cell = Cell as WFCCell_2D;

		OwnerCell = cell;

		//For each possible direction
		for (int i = 0; i < Enum.GetNames(typeof(CellDirection.Direction)).Length; i++)
		{
			//Test if we've set the flag for the given direction
			CellDirection.Direction currentDirection = (CellDirection.Direction)(Enum.GetValues(typeof(CellDirection.Direction))).GetValue(i);
			if (FlagsHelper.IsSet<CellDirection.Direction>(direction, currentDirection))
			{

				//If there is a cell in this direction add it to targetCells
				Vector2Int direction = cell.Position + CellDirection.CellDirectionToVector2Int(currentDirection);
				IWFCCell targetCell = m.GetCell(direction);
				if (targetCell != null)
				{
					// Debug.Log($"cellPos = {cell.Position} + {currentDirection} {CellDirection.CellDirectionToVector2Int(currentDirection)} = {direction} ");
					if (targetCells == null)
						targetCells = new System.Collections.Generic.List<IWFCCell>();

					if (cell.Position == new Vector2Int(2, 2))
					{
						Debug.Log($"Target Cell {((WFCCell_2D)targetCell).Position}");
					}
					targetCells.Add(targetCell);

					// When the target cell is updated cause our cell to do a domain check
					// targetCell.OnCellUpdate += TargetCellUpdated;
					targetCell.OnCellUpdate += (WFCCellUpdate u) => cell.DomainCheck();
				}
			}
		}

		if (cell.Position == new Vector2Int(2, 2) && targetCells != null)
		{
			Debug.Log("Printing target cells from (2,2)");
			foreach (IWFCCell target in targetCells)
			{
				Debug.Log($"Targeting Cell at position: {((WFCCell_2D)target).Position}");
			}
		}
		else if (cell.Position == new Vector2Int(2, 2) && targetCells == null)
		{
			Debug.Log("Target Cells is null");
		}


		// string toPrint = "Pos: " + cell.Position;

		// if (targetCells != null)
		// {
		// 	toPrint += ", TargetCells.Count " + targetCells.Count;
		// }
		// else
		// {
		// 	toPrint += ", No Target Cells.";
		// }

		// Debug.Log(toPrint);

	}
}