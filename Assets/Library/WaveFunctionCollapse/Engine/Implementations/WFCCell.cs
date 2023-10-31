using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public class WFCCell : IComparable
	{
		public WFCCellDomain Domain;
		public WFCTile CollapsedTile { get; protected set; }
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
			Domain.Domain = newDomain;
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
			if (DomainBitMaskID == 0) { throw new ImpossibleDomainException("Nothing left in cell's domain."); }
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
		}

		#region Domain

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