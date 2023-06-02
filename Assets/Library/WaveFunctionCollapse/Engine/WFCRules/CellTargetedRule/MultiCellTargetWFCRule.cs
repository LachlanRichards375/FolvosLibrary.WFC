using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		[SerializeField] public List<Vector2Int> targetCells;
		[SerializeField] public WFCTile goal;
		public CellDirection.Direction direction;

		protected MultiCellTargetWFCRule() : base()
		{

		}

		public MultiCellTargetWFCRule(MultiCellTargetWFCRule other) : base(other)
		{
			goal = other.goal;
			direction = other.direction;
		}

		public override void DrawRuleProperties()
		{
			goal = (WFCTile)EditorGUILayout.ObjectField("Goal Tile: ", goal, typeof(WFCTile), true);
			direction = (CellDirection.Direction)EditorGUILayout.EnumFlagsField("Directions: ", direction);
		}

		public override void RuleInitialize(IWFCManager manager, Vector2Int CellPos)
		{
			this.manager = manager;

			//For each possible direction
			foreach (CellDirection.Direction currentDirection in (CellDirection.Direction[])Enum.GetValues(typeof(CellDirection.Direction)))
			{

				//If there is a cell in this direction add it to targetCells
				Vector2Int direction = CellPos + CellDirection.CellDirectionToVector2Int(currentDirection);
				IWFCCell targetCell = manager.GetCell(new IWFCPosition(direction));
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

		protected IWFCCell GetTargetCell(Vector2Int pos)
		{
			return manager.GetCell(new IWFCPosition(pos));
		}
	}
}