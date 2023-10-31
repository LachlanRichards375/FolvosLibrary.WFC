using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{

	using System.Collections.Generic;

	public class WFCCellDomain
	{

		WFCCellDomain(WFCCell owner)
		{
			cell = owner;
		}

		WFCCell cell;

		public List<WFCTile> Domain { get => domain; set => SetDomain(value); }
		List<WFCTile> domain;
		private ulong DomainBitMaskID = 0;

		void SetDomain(List<WFCTile> newDomain)
		{
			domain = newDomain;
			DomainBitMaskID = 0;
			foreach (WFCTile t in Domain)
			{
				DomainBitMaskID |= t.ID;
			}
		}

		public WFCCellUpdate? DomainCheck(WFCCellUpdate update)
		{
			//If we've collapsed we don't care
			if (cell.CollapsedTile != null)
			{
				return null;
			}

			// List<WFCTile> tilesToRemove = new List<WFCTile>();
			ulong tilesToRemove = 0;
			int i = 0;
			foreach (WFCTile tile in Domain)
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
			updateMessage.DomainChanges = new List<DomainChange>();

			foreach (WFCTile t in Domain)
			{
				if ((t.ID & tilesToRemove) == t.ID)
				{
					updateMessage.DomainChanges.Add(new DomainChange(t, DomainUpdate.RemovedFromDomain));
				}
			}

			//Remove the bit from the bitmask;
			DomainBitMaskID &= ~tilesToRemove;

			return updateMessage;
		}

		protected int CalcDomain()
		{
			int sum = 0;
			foreach (WFCTile tile in Domain)
			{
				sum += tile.TileWeight;
			}
			return sum;
		}

		protected int GetActualDomainSize()
		{
			int domainSize = 0;
			if (Domain != null)
			{
				for (int i = 0; i < Domain.Count; i++)
				{
					if (Domain[i] != null)
					{
						domainSize++;
					}
				}
			}

			return domainSize;
		}
	}
}