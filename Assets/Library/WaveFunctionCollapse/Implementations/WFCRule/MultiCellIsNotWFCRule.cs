using System;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using UnityEngine;

[System.Serializable]
public class MultiCellIsNotTarget2D : MultiCellTargetWFCRule
{
	public MultiCellIsNotTarget2D() : base()
	{

	}

	public MultiCellIsNotTarget2D(MultiCellIsNotTarget2D other) : base(other)
	{

	}

	public WFCPosition[] GetTargetCellsArray()
	{

		List<WFCPosition> localDirections = new();
		foreach (CellDirection.Direction d in Enum.GetValues(typeof(CellDirection.Direction)))
		{
			if (direction.HasFlag(d))
			{
				localDirections.Add(new WFCPosition(CellDirection.CellDirectionToVector2Int(d)));
			}
		}

		foreach (var dir in targetCells)
		{
			localDirections.Add(dir);
		}

		string s = "localDirections:\n";
		foreach (var v in localDirections)
		{
			s += v.ToString() + ", ";
		}

		return localDirections.ToArray();
	}
}