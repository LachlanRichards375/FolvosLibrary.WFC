using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class IWFCCell : IComparable
	{
		public WFCTile CollapsedTile { get; protected set; }
		public WFCTile[] Domain;
		public event Action<WFCCellUpdate> OnCellUpdate;

		protected IWFCManager manager;

		public IWFCCell(IWFCManager manager)
		{
			this.manager = manager;
		}

		public void RuleSetup()
		{
			OnCellUpdate += (WFCCellUpdate update) => Debug.Log("Cell called OnCellUpdate: " + update.ToString());
			foreach (WFCTile tile in Domain)
			{
				tile.RuleSetup(manager, this);
			}
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
			//return domain Length without weighting
			return Domain.Length;
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

		public void DomainCheck()
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
				if (!tile.PassesRules())
				{
					// toRemove.Add(i);
					tilesToRemove.Add(tile);
				}
				i++;
			}

			if (tilesToRemove.Count > 0)
			{
				string toPrint = ("Attempting to remove " + tilesToRemove.Count + " tiles from domain(" + Domain.Length + ")");
				if (this is WFCCell_2D)
				{
					WFCCell_2D cell = this as WFCCell_2D;
					toPrint += (" in cell at position " + cell.Position);
				}
				Debug.Log(toPrint);

				WFCCellUpdate updateMessage = new WFCCellUpdate();

				updateMessage.UpdateType = CellUpdateType.DomainUpdate;
				updateMessage.UpdatedCell = this;

				for (i = 0; i < tilesToRemove.Count; i++)
				{
					updateMessage.DomainChanges.Add(new DomainChange(tilesToRemove[i], DomainUpdate.RemovedFromDomain));
					Debug.Log($"Removing {tilesToRemove.Count} tiles from domain: {Domain.Length}");
					//Remove tile
					Domain = RemoveFromDomain(tilesToRemove[i]);
				}

				OnCellUpdate.Invoke(updateMessage);
			}
		}

		WFCTile[] RemoveFromDomain(WFCTile toRemove)
		{
			List<WFCTile> returner = new List<WFCTile>(Domain);
			returner.Remove(toRemove);
			if (returner.Count == 0)
			{
				returner.Add(null);
			}
			return returner.ToArray();
		}

		WFCTile[] RemoveAt(int index)
		{
			if (index < 0 || index >= Domain.Length)
			{
				return null;
			}

			int size = Mathf.Max(Domain.Length - 1, 0);
			WFCTile[] returner = new WFCTile[size];

			int RulesCount = 0, returnerCount = 0;
			while (RulesCount < Domain.Length)
			{

				if (RulesCount != index)
				{
					returner[returnerCount] = Domain[RulesCount];
					returnerCount++;
				}

				RulesCount++;
			}

			return returner;
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

		public override abstract string ToString();

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