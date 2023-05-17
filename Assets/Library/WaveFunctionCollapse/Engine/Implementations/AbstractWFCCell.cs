using System;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class AbstractWFCCell : IWFCCell
	{

		public WFCTile CollapsedTile;
		public WFCTile[] Domain;
		protected int domainSum = 0;
		public event Action<WFCCellUpdate> OnCellUpdate;

		protected IWFCManager manager;

		public AbstractWFCCell(IWFCManager manager)
		{
			this.manager = manager;
			manager.OnInitialize += PreInitialize;
		}

		public float CalculateEntropy()
		{
			float entropy = 0.0f;
			foreach (WFCTile tile in Domain)
			{
				if (tile.TileWeight > 0)
				{
					entropy -= (tile.TileWeight / domainSum) * Mathf.Log10((tile.TileWeight / domainSum));
				}
			}
			return entropy;
		}

		public void Collapse()
		{
			float tileNo = UnityEngine.Random.Range(0f, domainSum);
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
				tile.RuleSetup(manager);
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
	}
}