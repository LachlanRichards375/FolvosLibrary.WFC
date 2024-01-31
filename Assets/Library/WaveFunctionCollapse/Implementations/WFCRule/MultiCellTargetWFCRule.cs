using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		public WFCPosition[] targetCells = new WFCPosition[0];
		public WFCTile goal;
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
	}
}