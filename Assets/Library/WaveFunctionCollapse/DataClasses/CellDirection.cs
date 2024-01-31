using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public class CellDirection
	{
		[Flags]
		public enum Direction
		{
			North = 1 << 0,
			NorthEast = 1 << 2,
			East = 1 << 3,
			SouthEast = 1 << 4,
			South = 1 << 5,
			SouthWest = 1 << 6,
			West = 1 << 7,
			NorthWest = 1 << 8,
			Invalid = 1 << 9
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

		public static Direction Vector2IntToCellDirection(Vector2Int direction)
		{
			//North/south
			if (direction.x == 0)
			{
				if (direction.y == 1)
				{
					return Direction.North;
				}
				else if (direction.y == -1)
				{
					return Direction.South;
				}
			}

			//West
			if (direction.x == -1)
			{
				if (direction.y == 1)
				{
					return Direction.NorthWest;
				}
				else if (direction.y == -1)
				{
					return Direction.SouthWest;
				}
				else if (direction.y == 0)
				{
					return Direction.West;
				}
			}

			//East
			if (direction.x == 1)
			{
				if (direction.y == 1)
				{
					return Direction.NorthEast;
				}
				else if (direction.y == -1)
				{
					return Direction.SouthEast;
				}
				else if (direction.y == 0)
				{
					return Direction.East;
				}
			}

			return Direction.Invalid;
		}
	}

}