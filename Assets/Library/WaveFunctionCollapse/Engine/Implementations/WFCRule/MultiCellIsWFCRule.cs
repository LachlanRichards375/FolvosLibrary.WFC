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
		bool[] PassTest = new bool[targetCells.Count];
		int i = 0;
		foreach (IWFCCell ICell in targetCells)
		{

			IWFCCell cell = (IWFCCell)ICell;

			//IF cell has collapsed and is equal to our goal
			if (cell.HasCollapsed())
			{
				PassTest[i] = cell.CollapsedTile == goal;
				continue;
			}

			//If our target's domain contains our goal
			for (int target = 0; target < cell.Domain.Length; target++)
			{
				if (cell.Domain[target] == goal)
				{
					PassTest[i] = true;
					break; //break out of for target loop, continue to next target cell
				}
			}

			i++;
		}

		if (PassTest.Any(e => e == true))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public override void RuleInitialize(IWFCManager manager, IWFCCell Cell)
	{
		WFCManager_2D m = manager as WFCManager_2D;
		WFCCell_2D cell = Cell as WFCCell_2D;
		for (int i = 0; i < Enum.GetNames(typeof(CellDirection.Direction)).Length; i++)
		{
			CellDirection.Direction currentDirection = (CellDirection.Direction)(Enum.GetValues(typeof(CellDirection.Direction))).GetValue(i);
			// Debug.Log("Loop");
			if (FlagsHelper.IsSet<CellDirection.Direction>(direction, currentDirection))
			{
				Vector2Int direction = cell.Position + CellDirection.CellDirectionToVector2Int(currentDirection);
				Debug.Log($"cellPos = {cell.Position} + {currentDirection} ({CellDirection.CellDirectionToVector2Int(currentDirection)}) = {direction} ");
				// IWFCCell targetCell = m.GetCell(direction);
				// Debug.Log($"Target Cell {targetCell}");
				// 	targetCells.Add(targetCell);
			}
		}
	}

}