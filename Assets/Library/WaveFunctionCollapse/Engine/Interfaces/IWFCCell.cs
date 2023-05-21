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
			//return number of tiles - domain Length + 1 because 0 is bad
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

		public void DomainCheck(bool fromRule = false)
		{
			if (fromRule)
			{
				Debug.Log("Domain check was called from a rule");
			}

			List<int> toRemove = new List<int>();
			int i = 0;
			foreach (WFCTile tile in Domain)
			{
				if (!tile.PassesRules())
				{
					toRemove.Add(i);
				}
				i++;
			}

			if (fromRule && toRemove.Count > 0)
			{
				Debug.Log("Attempting to remove " + toRemove.Count + " tiles from domain");
			}

			for (i = 0; i < toRemove.Count; i++)
			{
				Domain = RemoveAt(toRemove[i] - i);
			}
		}

		WFCTile[] RemoveAt(int index)
		{
			if (index < 0 || index >= Domain.Length)
			{
				return null;
			}

			WFCTile[] returner = new WFCTile[Domain.Length - 1];

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