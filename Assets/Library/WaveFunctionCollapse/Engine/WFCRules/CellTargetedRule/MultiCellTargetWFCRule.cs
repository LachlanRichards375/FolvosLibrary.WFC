using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		// protected List<Vector2Int> targetCells = new List<Vector2Int>();
		protected Vector2Int[] targetCells = new Vector2Int[0];
		public WFCTile goal;
		public CellDirection.Direction direction;

		protected MultiCellTargetWFCRule() : base()
		{

		}

		public MultiCellTargetWFCRule(MultiCellTargetWFCRule other) : base(other)
		{
			targetCells = (Vector2Int[])other.targetCells.Clone();
			goal = other.goal;
			direction = other.direction;
		}

		public override void DrawRuleProperties()
		{
			base.DrawRuleProperties();
			goal = (WFCTile)EditorGUILayout.ObjectField("Goal Tile: ", goal, typeof(WFCTile), true);
			direction = (CellDirection.Direction)EditorGUILayout.EnumFlagsField("Directions: ", direction);
		}

		public override void RuleInitialize(IWFCManager manager, Vector2Int CellPos)
		{
			this.manager = manager;

			List<Vector2Int> targetCells = new List<Vector2Int>();
			//For each possible direction
			foreach (CellDirection.Direction currentDirection in (CellDirection.Direction[])Enum.GetValues(typeof(CellDirection.Direction)))
			{

				//If there is a cell in this direction add it to targetCells
				Vector2Int direction = CellPos + CellDirection.CellDirectionToVector2Int(currentDirection);
				IWFCCell targetCell = manager.GetCell(new IWFCPosition(direction));
				if (targetCell is null)
				{
					continue;
				}

				if (FlagsHelper.IsSet<CellDirection.Direction>(this.direction, currentDirection))
				{
					targetCells.Add(direction);
					// When the target cell is updated cause our cell to do a domain check
					targetCell.OnCellUpdate += InvokeRuleActivated;
				}
			}

			this.targetCells = targetCells.ToArray();

			// PrintCellTargets(CellPos);
		}

		public void PrintCellTargets(Vector2Int cellPos)
		{
			string s = $"{goal.Name} at {cellPos} has {targetCells.Length} target cells: ";
			foreach (Vector2Int v in targetCells)
			{
				s += $"{v}, ";
			}

			Debug.Log(s);
		}

		public override string GetTargetCells()
		{
			string s = "";
			for (int i = 0; i < targetCells.Length; i++)
			{
				s += $"{targetCells[i]}, ";
			}
			return s;
		}

		protected IWFCCell GetTargetCell(Vector2Int pos)
		{
			return manager.GetCell(new IWFCPosition(pos));
		}
	}
}