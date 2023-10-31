using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public class WFCCell : IComparable
	{
		public WFCTile CollapsedTile { get; protected set; }
		public List<WFCTile> Domain { get; protected set; }
		private ulong DomainBitMaskID = 0;
		public event Action<WFCCellUpdate> OnCellUpdate;

		protected IWFCManager manager;
		protected WFCPosition position;

		public WFCCell(IWFCManager m, WFCPosition p)
		{
			manager = m;
			position = p;
		}

		public WFCCell(WFCCell other)
		{
			this.manager = other.manager;
			this.position = other.position;
			this.Domain = other.Domain;
			this.CollapsedTile = other.CollapsedTile;
		}

		public void SetDomain(List<WFCTile> newDomain)
		{
			Domain = newDomain;
			DomainBitMaskID = 0;
			foreach (WFCTile t in Domain)
			{
				DomainBitMaskID |= t.ID;
			}
		}

		public void RuleSetup()
		{
			foreach (WFCTile tile in Domain)
			{
				WFCCell local = this;
				tile.RuleSetup(manager, local);
			}
		}

		public float CalculateEntropy()
		{
			//return domain Length without weighting
			// return Domain.Count;
			float domainWeight = 0;
			for (int i = 0; i < Domain.Count; i++)
			{
				domainWeight += Domain[i].TileWeight;
			}
			return domainWeight;
		}

		public WFCCellUpdate Collapse()
		{
			if (Domain.Count <= 0) { throw new ImpossibleDomainException("Nothing left in cell's domain."); }
			float tileNo = UnityEngine.Random.Range(0f, CalcDomain());
			int index = 0;
			for (index = 0; index < Domain.Count; index++)
			{
				tileNo -= Domain[index].TileWeight;
				if (tileNo <= 0f)
				{
					break;
				}
			}
			if (index >= Domain.Count) index = Domain.Count - 1;
			return Collapse(Domain[index]);
		}

		public WFCCellUpdate Collapse(WFCTile toCollapseTo)
		{
			CollapsedTile = toCollapseTo;

			WFCCellUpdate updateMessage = new WFCCellUpdate();

			updateMessage.UpdateType = CellUpdateType.Collapsed;
			updateMessage.UpdatedCell = this;

			return updateMessage;
			// InvokeCellUpdate(updateMessage);
		}

		#region Domain

		public WFCCellUpdate? DomainCheck(WFCCellUpdate update)
		{
			//If we've collapsed we don't care
			if (CollapsedTile != null)
			{
				return null;
			}

			// List<WFCTile> tilesToRemove = new List<WFCTile>();
			ulong tilesToRemove = 0;
			int i = 0;
			foreach (WFCTile tile in Domain)
			{
				if (!tile.PassesRules(update, this))
				{
					tilesToRemove |= tile.ID;
				}
				i++;
			}

			if (tilesToRemove == DomainBitMaskID && CollapsedTile != null)
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
			updateMessage.UpdatedCell = this;
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

		#endregion

		public virtual string GetPositionString()
		{
			return position.ToString();
		}

		public virtual WFCPosition GetPosition()
		{
			return new WFCPosition(position);
		}

		public int CompareTo(object obj)
		{
			if (obj == null) return 1;
			WFCCell otherTile = obj as WFCCell;
			if (otherTile == null) throw new ArgumentException("Object is not a WFCCell");

			return this.CalculateEntropy().CompareTo(otherTile.CalculateEntropy());
		}

		public int Compare(object x, object y)
		{
			if (x == null || y == null) return 1;
			WFCCell otherCell = x as WFCCell;

			if (otherCell == null) throw new ArgumentException("Object is not a WFCCell");

			return otherCell.CompareTo(y);
		}

		public int CellUpdateListeners()
		{
			return OnCellUpdate.GetInvocationList().Length;
		}

		public override String ToString()
		{
			if (CollapsedTile != null)
			{
				return CollapsedTile.Name;
			}

			string returner = "Undecided (" + GetActualDomainSize() + ")";
			if (Domain == null)
			{
				// Debug.Log("Domain is null");
				returner += "  NULL?  ";
			}
			else
			{
				foreach (WFCTile tile in Domain)
				{
					returner += tile.Name + " ";
				}
			}
			return returner;
		}

		public WFCCellStruct GetCellStruct()
		{
			return new WFCCellStruct(CollapsedTile, Domain);
		}
	}

	[System.Serializable]
	public struct WFCCellStruct
	{
		public WFCTile CollapsedTile;
		public List<WFCTile> Domain;

		public WFCCellStruct(WFCTile collapsedTile, List<WFCTile> domain)
		{
			CollapsedTile = collapsedTile;
			Domain = domain;
		}
	}
}