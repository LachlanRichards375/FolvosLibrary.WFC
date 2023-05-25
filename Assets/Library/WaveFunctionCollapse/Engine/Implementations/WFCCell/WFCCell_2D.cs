
using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public class WFCCell_2D : IWFCCell
	{
		public WFCCell_2D(IWFCManager manager, Vector2Int Position) : base(manager)
		{
			this.Position = Position;
		}

		public WFCCell_2D(WFCCell_2D other) : base(other.manager)
		{
			this.Position = other.Position;
			this.Domain = other.Domain;
			this.CollapsedTile = other.CollapsedTile;
		}

		public Vector2Int Position;

		public override WFCError GetError()
		{
			WFCError e = new WFCError();
			e.Message = $"Error on cell at position {Position}, Domain has {Domain.Length} remaining elements";
			return e;
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

		int GetActualDomainSize()
		{
			int domainSize = 0;
			if (Domain != null)
			{
				for (int i = 0; i < Domain.Length; i++)
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