
using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public class WFCCell_2D : IWFCCell
	{
		public WFCCell_2D(IWFCManager manager) : base(manager)
		{
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
			if (CollapsedTile == null)
			{
				string returner = "Undecided ";
				foreach (WFCTile tile in Domain)
				{
					returner += tile.Name + " ";
				}
				return returner;
			}
			return CollapsedTile.Name;
		}
	}
}