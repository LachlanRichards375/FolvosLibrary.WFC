using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public class IWFCCell : IComparable
	{
		public WFCTile CollapsedTile { get; protected set; }
		public List<WFCTile> Domain;
		public event Action<WFCCellUpdate> OnCellUpdate;

		protected IWFCManager manager;
		protected WFCPosition position;

		public IWFCCell(IWFCManager m, WFCPosition p)
		{
			manager = m;
			position = p;
		}

		public IWFCCell(IWFCCell other)
		{
			this.manager = other.manager;
			this.position = other.position;
			this.Domain = other.Domain;
			this.CollapsedTile = other.CollapsedTile;
		}

		public void RuleSetup()
		{
			foreach (WFCTile tile in Domain)
			{
				IWFCCell local = this;
				tile.RuleSetup(manager, local);
			}
		}

		public float CalculateEntropy()
		{
			//return domain Length without weighting
			return Domain.Count;
		}

		public WFCCellUpdate Collapse()
		{
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

		public WFCCellUpdate? DomainCheck(WFCCellUpdate update)
		{
			//If we've collapsed we don't care
			if (CollapsedTile != null)
			{
				return null;
			}

			List<WFCTile> tilesToRemove = new List<WFCTile>();
			int i = 0;
			foreach (WFCTile tile in Domain)
			{
				if (!tile.PassesRules(update, this))
				{
					tilesToRemove.Add(tile);
				}
				i++;
			}

			if (tilesToRemove.Count == Domain.Count)
			{
				Debug.LogError("Removed all tiles from a cells domain");
			}

			return RemoveFromDomain(tilesToRemove);
		}

		WFCCellUpdate? RemoveFromDomain(List<WFCTile> tilesToRemove)
		{
			if (tilesToRemove.Count <= 0)
			{
				return null;
			}
			int i = 0;
			WFCCellUpdate updateMessage = new WFCCellUpdate();

			updateMessage.UpdateType = CellUpdateType.DomainUpdate;
			updateMessage.UpdatedCell = this;
			if (tilesToRemove.Count > 0)
			{
				updateMessage.DomainChanges = new List<DomainChange>();
			}

			for (i = 0; i < tilesToRemove.Count; i++)
			{
				updateMessage.DomainChanges.Add(new DomainChange(tilesToRemove[i], DomainUpdate.RemovedFromDomain));
				//Remove tile
			}

			for (i = 0; i < tilesToRemove.Count; i++)
			{
				Domain.Remove(tilesToRemove[i]);
			}
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

		public virtual WFCError GetError()
		{
			WFCError e = new WFCError();
			e.Message = $"Error on cell at position {position}, Domain has {Domain.Count} remaining elements";
			return e;
		}

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
			IWFCCell otherTile = obj as IWFCCell;
			if (otherTile == null) throw new ArgumentException("Object is not a FWCTile");

			return this.CalculateEntropy().CompareTo(otherTile.CalculateEntropy());
		}

		public int Compare(object x, object y)
		{
			if (x == null || y == null) return 1;
			IWFCCell otherCell = x as IWFCCell;

			if (otherCell == null) throw new ArgumentException("Object is not a FWCTile");

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