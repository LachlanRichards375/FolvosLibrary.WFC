using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{


	public class CellDirection
	{
		[Flags]
		public enum Direction
		{
			North = 1,
			NorthEast = 2,
			East = 3,
			SouthEast = 4,
			South = 5,
			SouthWest = 6,
			West = 7,
			NorthWest = 8
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