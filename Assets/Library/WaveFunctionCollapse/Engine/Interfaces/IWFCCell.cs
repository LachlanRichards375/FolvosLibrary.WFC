using System;
using System.Collections;
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
		protected IWFCPosition position;

		public IWFCCell(IWFCManager m, IWFCPosition p)
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

		public void Collapse()
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
			CollapsedTile = Domain[index];

			WFCCellUpdate updateMessage = new WFCCellUpdate();

			updateMessage.UpdateType = CellUpdateType.Collapsed;
			updateMessage.UpdatedCell = this;

			InvokeCellUpdate(updateMessage);
		}

		public void DomainCheck(WFCCellUpdate update)
		{
			//If we've collapsed we don't care
			if (CollapsedTile != null)
			{
				return;
			}

			List<WFCTile> tilesToRemove = new List<WFCTile>();
			int i = 0;
			foreach (WFCTile tile in Domain)
			{
				if (!tile.PassesRules(update, this))
				{
					Debug.Log($"Testing {tile.Name} at {GetPositionString()} returned False");
					tilesToRemove.Add(tile);
				}
				i++;
			}

			RemoveFromDomain(tilesToRemove);
		}

		void RemoveFromDomain(List<WFCTile> tilesToRemove)
		{
			if (tilesToRemove.Count > 0)
			{
				int i = 0;
				// Debug.Log($"Attempting to remove {tilesToRemove.Count} tiles from domain({Domain.Count}) at {position.ToString()}");

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
				InvokeCellUpdate(updateMessage);
			}
		}

		protected void InvokeCellUpdate(WFCCellUpdate update)
		{
			OnCellUpdate.Invoke(update);
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

		public virtual IWFCPosition GetPosition()
		{
			return new IWFCPosition(position);
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
	}
}