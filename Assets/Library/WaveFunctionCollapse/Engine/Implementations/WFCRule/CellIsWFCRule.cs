using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

public class CellIsTarget2D : CellTargetWFCRule
{
	[SerializeField] public WFCTile goal;
	public Vector2Int offset;

	public CellIsTarget2D(CellIsTarget2D rule) : base(rule)
	{
		goal = rule.goal;
		offset = rule.offset;
	}

	public override void DrawRuleProperties()
	{
		EditorGUILayout.LabelField("Drawing from WFCRule");
	}

	public override bool Test()
	{
		Debug.LogError("Test() is not implemented for CellIsWFCRule.cs 22");
		return false;
	}

	public override bool Test(WFCCellUpdate? cellUpdate, IWFCCell OwnerCell)
	{
		IWFCCell cell = (IWFCCell)targetCell;

		//IF cell has collapsed and is equal to our goal
		if (cell.CollapsedTile != null)
		{
			return cell.CollapsedTile == goal;
		}

		//If our target's domain contains our goal
		for (int i = 0; i < cell.Domain.Count; i++)
		{
			if (cell.Domain[i] == goal)
			{
				return true;
			}
		}

		return false;
	}

	public override void RuleInitialize(IWFCManager manager, Vector2Int CellPos)
	{
		// WFCManager_2D m = manager as WFCManager_2D;
		// WFCCell_2D cell = Cell as WFCCell_2D;
		// targetCell = m.GetCell(offset + cell.Position);
	}
}