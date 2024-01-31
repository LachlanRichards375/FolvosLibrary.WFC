using System.Collections.Generic;

namespace FolvosLibrary.WFC
{
	public struct WFCCellUpdate
	{
		public CellUpdateType UpdateType;
		public WFCCell UpdatedCell;
		public ulong RemovedFromDomain;
		public ulong AddedToDomain;
		// public List<DomainChange> DomainChanges;

		public override string ToString()
		{
			string s = $"CellUpdateType: {UpdateType} at {UpdatedCell.GetPositionString()}";

			if(RemovedFromDomain > 0){
				foreach (WFCTile updatedTile in WFCCellDomain.GetTilesFromGlobalDomain(RemovedFromDomain))
				{
					s += " tile: " + updatedTile.Name + " was removed from the domain.";
				}
			}

			if(AddedToDomain > 0){
				foreach (WFCTile updatedTile in WFCCellDomain.GetTilesFromGlobalDomain(RemovedFromDomain))
				{
					s += " tile: " + updatedTile.Name + " was added to the domain";
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
