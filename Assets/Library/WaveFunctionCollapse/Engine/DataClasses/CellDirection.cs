using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{


	public class CellDirection
	{
		[Flags]
		public enum Direction
		{
			Nothing = 0,
			Everything = ~0,
			North = 1 << 0,
			NorthEast = 1 << 2,
			East = 1 << 3,
			SouthEast = 1 << 4,
			South = 1 << 5,
			SouthWest = 1 << 6,
			West = 1 << 7,
			NorthWest = 1 << 8,
			AllNorth = NorthWest | North | NorthEast,
			AllEast = NorthEast | East | SouthEast,
			AllSouth = SouthWest | South | SouthEast,
			AllWest = SouthWest | West | NorthWest,
		}

		public static Vector2Int CellDirectionToVector2Int(Direction direction)
		{
			switch (direction)
			{
				case Direction.North:
					return new Vector2Int(0, 1);
				case Direction.NorthEast:
					return new Vector2Int(1, 1);
				case Direction.East:
					return new Vector2Int(1, 0);
				case Direction.SouthEast:
					return new Vector2Int(1, -1);
				case Direction.South:
					return new Vector2Int(0, -1);
				case Direction.SouthWest:
					return new Vector2Int(-1, -1);
				case Direction.West:
					return new Vector2Int(-1, 0);
				case Direction.NorthWest:
					return new Vector2Int(-1, 1);
			}
			return new Vector2Int(0, 0);
		}
	}

}