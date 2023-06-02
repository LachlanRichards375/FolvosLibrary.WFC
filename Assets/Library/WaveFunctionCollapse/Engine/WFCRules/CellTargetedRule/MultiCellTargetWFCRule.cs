using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class MultiCellTargetWFCRule : WFCRule
	{
		[SerializeField] public List<Vector2Int> targetCells;

		protected MultiCellTargetWFCRule() : base()
		{

		}

		protected MultiCellTargetWFCRule(WFCRule rule) : base(rule)
		{
		}
	}
}