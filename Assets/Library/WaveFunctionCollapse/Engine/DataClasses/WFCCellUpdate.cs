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
	}

	public enum CellUpdateType
	{
		Collapsed,
		DomainUpdate
	}

	public struct DomainChange
	{
		WFCTile UpdatedTile;
		DomainUpdate DomainUpdate;
	}

	public enum DomainUpdate
	{
		RemovedFromDomain,
		AddedToDomain,
	}
}
