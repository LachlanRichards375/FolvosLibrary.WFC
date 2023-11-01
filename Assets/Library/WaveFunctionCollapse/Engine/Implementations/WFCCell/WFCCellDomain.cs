using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{

	using System.Collections.Generic;

	public class WFCCellDomain
	{
		public static WFCTile[] GlobalDomain;

		public static WFCTile[] GetTilesFromGlobalDomain(ulong bitmask)
		{
			if (bitmask == 0) { return null; }

			List<WFCTile> returner = new List<WFCTile>();
			foreach (WFCTile tile in GlobalDomain)
			{
				if ((tile.ID & bitmask) == tile.ID)
				{
					returner.Add(tile);
				}
			}
			return returner.ToArray();
		}

		public WFCCellDomain(WFCCell owner)
		{
			cell = owner;
		}

		WFCCell cell;
		public ulong DomainBitMaskID { get; private set; } = 0;
		public void SetDomain(IEnumerable<WFCTile> newDomain)
		{
			DomainBitMaskID = 0;
			foreach (WFCTile t in newDomain)
			{
				DomainBitMaskID |= t.ID;
			}
		}

		public WFCTile[] GetTileArray()
		{
			return GetTilesFromGlobalDomain(DomainBitMaskID);
		}


		public WFCCellUpdate? DomainCheck(WFCCellUpdate update)
		{
			//If we've collapsed we don't care
			if (cell.CollapsedTile != null)
			{
				return null;
			}

			ulong tilesToRemove = 0;
			int i = 0;
			foreach (WFCTile tile in GetTilesFromGlobalDomain(DomainBitMaskID))
			{
				if (!tile.PassesRules(update, cell))
				{
					tilesToRemove |= tile.ID;
				}
				i++;
			}

			if (tilesToRemove == DomainBitMaskID && cell.CollapsedTile != null)
			{
				Debug.LogError("Removed all tiles from a cells domain");
				throw new ImpossibleDomainException("Contradiction detected.");
			}

			return RemoveFromDomain(tilesToRemove);
		}

		public bool DomainContains(ulong tileIDToCheck)
		{
			return (DomainBitMaskID & tileIDToCheck) == tileIDToCheck;
		}

		WFCCellUpdate? RemoveFromDomain(ulong tilesToRemove)
		{
			if (tilesToRemove == 0)
			{
				return null;
			}
			WFCCellUpdate updateMessage = new WFCCellUpdate();

			updateMessage.UpdateType = CellUpdateType.DomainUpdate;
			updateMessage.UpdatedCell = cell;
			updateMessage.RemovedFromDomain = tilesToRemove;

			//Remove the bit from the bitmask;
			DomainBitMaskID &= ~tilesToRemove;

			return updateMessage;
		}

		public int MaxTileWeight
		{
			get
			{
				int sum = 0;
				foreach (WFCTile tile in GetTilesFromGlobalDomain(DomainBitMaskID))
				{
					sum += tile.TileWeight;
				}
				return sum;
			}
		}

		public int Count
		{
			get
			{
				ulong n = DomainBitMaskID;
				int count = 0;
				while (n != 0)
				{
					count++;
					n &= (n - 1);
				}
				return count;
			}
		}
	}
}