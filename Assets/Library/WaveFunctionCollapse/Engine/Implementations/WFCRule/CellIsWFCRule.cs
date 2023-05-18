using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

public class CellIsTarget2D : CellTargetWFCRule
{
	[SerializeField] public WFCTile goal;
	public Vector2Int offset;

	public override void DrawRuleProperties()
	{
		EditorGUILayout.LabelField("Drawing from WFCRule");
	}

	public override bool Test()
	{
		IWFCCell cell = (IWFCCell)targetCell;

		//IF cell has collapsed and is equal to our goal
		if (cell.HasCollapsed())
		{
			return cell.CollapsedTile == goal;
		}

		//If our target's domain contains our goal
		for (int i = 0; i < cell.Domain.Length; i++)
		{
			if (cell.Domain[i] == goal)
			{
				return true;
			}
		}

		return false;
	}

	public override void RuleInitialize(IWFCManager manager, IWFCCell Cell)
	{
		WFCManager_2D m = manager as WFCManager_2D;
		WFCCell_2D cell = Cell as WFCCell_2D;
		targetCell = m.GetCell(offset + cell.Position);
	}
}