using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class IWFCCell : IComparable
	{
		public WFCTile CollapsedTile { get; protected set; }
		public List<WFCTile> Domain;
		public event Action<WFCCellUpdate> OnCellUpdate;

		protected IWFCManager manager;

		public IWFCCell(IWFCManager manager)
		{
			this.manager = manager;
		}

		public void RuleSetup()
		{
			// OnCellUpdate += (WFCCellUpdate update) => Debug.Log("Cell called OnCellUpdate: " + update.ToString());
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
				if (!tile.PassesRules(update))
				{
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
				string toPrint = ("Attempting to remove " + tilesToRemove.Count + " tiles from domain(" + Domain.Count + ")");
				if (this is WFCCell_2D)
				{
					WFCCell_2D cell = this as WFCCell_2D;
					toPrint += (" in cell at position " + cell.Position);
				}
				// Debug.Log(toPrint);

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

		public abstract WFCError GetError();

		public abstract string GetPosition();

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
	}
}