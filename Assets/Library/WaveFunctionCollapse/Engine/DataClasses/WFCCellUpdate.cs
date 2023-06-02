using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public struct WFCCellUpdate
	{
		public CellUpdateType UpdateType;
		public IWFCCell UpdatedCell;
		public List<DomainChange> DomainChanges;

		public override string ToString()
		{
			string s = $"CellUpdateType: {UpdateType} at {UpdatedCell.GetPositionString()}";

			if (DomainChanges != null && DomainChanges.Count > 0)
			{
				foreach (DomainChange change in DomainChanges)
				{
					s += " tile: " + change.UpdatedTile + " was " + change.DomainUpdate;
				}
			}

			return s;
		}
	}

	public enum CellUpdateType
	{
		Collapsed,
		DomainUpdate
	}

	public struct DomainChange
	{
		public DomainChange(WFCTile tile, DomainUpdate update)
		{
			UpdatedTile = tile;
			DomainUpdate = update;
		}
		public WFCTile UpdatedTile;
		public DomainUpdate DomainUpdate;
	}

	public enum DomainUpdate
	{
		RemovedFromDomain,
		AddedToDomain,
	}
}
