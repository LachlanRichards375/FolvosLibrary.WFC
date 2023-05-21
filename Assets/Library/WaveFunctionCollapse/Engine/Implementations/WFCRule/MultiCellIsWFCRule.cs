using System;
using System.Linq;
using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

public class MultiCellIsTarget2D : MultiCellTargetWFCRule
{
	[SerializeField] public WFCTile goal;
	public CellDirection.Direction direction;

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

			IWFCCell targetCell = (IWFCCell)ICell;

			//IF cell has collapsed and is equal to our goal
			if (targetCell.CollapsedTile != null)
			{
				PassTest[i] = targetCell.CollapsedTile == goal;
				continue;
			}

			PassTest[i] = false;
			//If our target's domain contains our goal
			for (int target = 0; target < targetCell.Domain.Length; target++)
			{
				if (targetCell.Domain[target] == goal)
				{
					PassTest[i] = true;
					break; //break out of for target loop, continue to next target cell
				}
			}

			i++;
		}

		//if any are false return false
		return !PassTest.Any(b => b == false);
	}

	public override void RuleInitialize(IWFCManager manager, IWFCCell Cell)
	{
		WFCManager_2D m = manager as WFCManager_2D;
		WFCCell_2D cell = Cell as WFCCell_2D;
		for (int i = 0; i < Enum.GetNames(typeof(CellDirection.Direction)).Length; i++)
		{
			CellDirection.Direction currentDirection = (CellDirection.Direction)(Enum.GetValues(typeof(CellDirection.Direction))).GetValue(i);
			if (FlagsHelper.IsSet<CellDirection.Direction>(direction, currentDirection))
			{
				Vector2Int direction = cell.Position + CellDirection.CellDirectionToVector2Int(currentDirection);
				// Debug.Log($"cellPos = {cell.Position} + {currentDirection} ({CellDirection.CellDirectionToVector2Int(currentDirection)}) = {direction} ");
				IWFCCell targetCell = m.GetCell(direction);
				// Debug.Log($"Target Cell {targetCell}");
				if (targetCell != null)
				{
					if (targetCells == null)
						targetCells = new System.Collections.Generic.List<IWFCCell>();
					targetCells.Add(targetCell);

					// targetCell.OnCellUpdate += TargetCellUpdated;
					targetCell.OnCellUpdate += (WFCCellUpdate u) => cell.DomainCheck(true);
				}
			}
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