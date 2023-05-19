using System;
using System.Collections;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class IWFCCell : IComparable, IComparer
	{
		public WFCTile CollapsedTile;
		public WFCTile[] Domain;
		public event Action<WFCCellUpdate> OnCellUpdate;

		protected IWFCManager manager;

		public IWFCCell(IWFCManager manager)
		{
			this.manager = manager;
			manager.OnInitialize += PreInitialize;
		}

		public float CalculateEntropy()
		{
			// float entropy = calcDomain();
			// foreach (WFCTile tile in Domain)
			// {
			// 	if (tile.TileWeight > 0)
			// 	{
			// 		entropy -= (tile.TileWeight / calcDomain()) * Mathf.Log10((tile.TileWeight / calcDomain()));
			// 	}
			// }
			//return number of tiles - domain Length + 1 because 0 is bad
			return manager.GetDomain().Length - Domain.Length + 1;
		}

		public void Collapse()
		{
			float tileNo = UnityEngine.Random.Range(0f, calcDomain());
			int index = 0;
			for (index = 0; index < Domain.Length; index++)
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

			OnCellUpdate?.Invoke(updateMessage);
		}

		public bool HasCollapsed()
		{
			return CollapsedTile == null;
		}

		private void PreInitialize()
		{
			OnCellUpdate += CellUpdated;

			Domain = manager.GetDomain();

			foreach (WFCTile tile in Domain)
			{
				tile.RuleSetup(manager, this);
			}
		}

		void CellUpdated(WFCCellUpdate update)
		{
			if (update.UpdateType != CellUpdateType.DomainUpdate)
			{
				calcDomain();
			}
		}

		protected int calcDomain()
		{
			int sum = 0;
			foreach (WFCTile tile in Domain)
			{
				sum += tile.TileWeight;
			}
			return sum;
		}

		public abstract WFCError GetError();

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

		public override abstract string ToString();
	}
}