using System;
using System.Collections.Generic;
using FolvosLibrary.Logging;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/Grids/2DGrid"), System.Serializable]
	public class WFCGrid2D : ScriptableObject, IWFCGrid
	{
		IWFCManager manager;
		public void SetManager(IWFCManager manager)
		{
			this.manager = manager;
		}


		IWFCPosition size;
		IWFCCell[][] grid = new IWFCCell[0][];

		public WFCError? Collapse()
		{
			IWFCCell cell = EntropyQueue[0];
			Vector2Int nextTile = cell.GetPosition().AsVector2Int();

			if (cell.CalculateEntropy() <= 0f)
			{
				//We have an issue
				Debug.LogError("Entropy queue.next is <= 0");
				return grid[nextTile.x][nextTile.y].GetError();
			}

			cell.Collapse();
			EntropyQueue.Remove(cell);
			SortQueue();

			return null;
		}

		public WFCError? CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo)
		{
			if (toCollapseTo == null)
			{
				Debug.LogError("Collapsing specific cell to null object");
				return null;
			}

			Vector2Int pos = position.AsVector2Int();

			if (pos.x < 0 || pos.x >= grid.Length)
			{
				// return WFCError out of bouds
				Debug.LogError("Position out of bounds for manual collapse");
				return null;
			}

			if (pos.y < 0 || pos.y >= grid[0].Length)
			{
				// return WFCError out of bouds
				Debug.LogError("Position out of bounds for manual collapse");
				return null;
			}

			IWFCCell toCollapse = grid[pos.x][pos.y];

			toCollapse.Collapse(toCollapseTo);

			EntropyQueue.Remove(toCollapse);

			return null;
		}

		public void SetSize(IWFCPosition size)
		{
			Vector2Int newSize = size.AsVector2Int();
			if (newSize.x < 0 || newSize.y < 0)
			{
				return;
			}

			IWFCCell[][] newGrid = new IWFCCell[newSize.x][];
			for (int x = 0; x < newSize.x; x++)
			{
				newGrid[x] = new IWFCCell[newSize.y];

				for (int y = 0; y < newSize.y; y++)
				{
					newGrid[x][y] = new IWFCCell(manager, new IWFCPosition(x, y));
				}
			}

			grid = newGrid;
			this.size = size;
		}

		protected void LoadGrid()
		{
			Debug.Log("Getting Entropy Queue");
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					IWFCCell cell = grid[x][y];
					EntropyQueue.Add(cell);
					cell.Domain = manager.GetDomain();
					cell.OnCellUpdate += (WFCCellUpdate) => SortQueue();
				}
			}

			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					IWFCCell cell = grid[x][y];
					cell.RuleSetup();
				}
			}

			ShuffleLowestEntropy();
		}

		public void Initialize()
		{
			int yLength = -1;
			if (grid.Length > 0)
			{
				yLength = grid[0].Length;
			}

			Debug.Log("grid size: [" + grid.Length + "," + yLength + "]");

			LoadGrid();

			CollapseFirstCellToSand();
		}

		void CollapseFirstCellToSand()
		{
			Debug.LogWarning("Force Collapsing first cell to sand");
			WFCTile collapseTo = null;
			foreach (WFCTile tile in manager.GetDomain())
			{
				if (tile.Name == "Sand")
				{
					collapseTo = tile;
					break;
				}
			}
			CollapseSpecificCell(EntropyQueue[0].GetPosition(), collapseTo);
		}

		public void DrawSize(bool ForceReset = false)
		{
			Vector2Int newSize = UnityEditor.EditorGUILayout.Vector2IntField("Map Size", size.AsVector2Int());
			if (ForceReset || newSize != size.AsVector2Int())
			{
				SetSize(new IWFCPosition(newSize));
			}
		}

		public IWFCCell[][] GetCells()
		{
			return grid;
		}

		public IWFCCell GetCell(IWFCPosition position)
		{
			if (position.x < 0 || position.y < 0)
			{
				return null;
			}

			if (position.x >= grid.Length || position.y >= grid[0].Length)
			{
				return null;
			}

			Vector2Int vec = position.AsVector2Int();
			return grid[vec.x][vec.y];
		}

		public bool HasCollapsed(IWFCPosition position)
		{
			if (position.x < 0 || position.y < 0)
			{
				return false;
			}

			if (position.x >= grid.Length || position.y >= grid[0].Length)
			{
				return false;
			}

			Vector2Int vec = position.AsVector2Int();
			return !(grid[vec.x][vec.y].CollapsedTile is null);
		}

		public void PrintCells()
		{
			Logging.Logging.LogMessage message = new Logging.Logging.LogMessage();

			message.MessageFrom = Logging.Logging.ProjectGroups.WFCManager;
			message.Priority = Logging.Logging.Priority.Low;

			if (Logging.Logging.CanLog(message))
			{
				string s = $"WFC Print Cells(): \n" +
				$"\t> Grid size: ({grid.Length},{grid[0].Length}), Entropy Queue.Count: {EntropyQueue.Count}\n";

				//This needs to be set out in this order so it
				//prints to the console the same direction as the exporter output
				for (int row = grid.Length - 1; row >= 0; row--)
				{
					string toAppend = $"{row}\t>";
					for (int column = 0; column < grid[0].Length; column++)
					{
						toAppend += String.Format("{0,50}", grid[column][row].ToString());
					}
					s += toAppend + "\n";
				}

				message.Message = s;
				Logging.Logging.Message(message);

				PrintEntropyQueue();
			}
		}

		public void PrintEntropyQueue()
		{
			Logging.Logging.LogMessage message = new Logging.Logging.LogMessage();

			message.MessageFrom = Logging.Logging.ProjectGroups.WFCManager;
			message.Priority = Logging.Logging.Priority.Low;

			string s = $"> Entropy Queue: \n";

			int i = 0;
			foreach (IWFCCell cell in EntropyQueue)
			{
				s += i + ">\t" + cell.GetPosition() + " " + cell.ToString() + "\n";
				i++;
			}

			message.Message = s;
			Logging.Logging.Message(message);
		}

		public IWFCPosition GetSize()
		{
			if (grid == null || grid[0] == null)
			{
				return new IWFCPosition(0, 0);
			}

			return new IWFCPosition(grid.Length, grid[0].Length);
		}

		//Entropy Queue
		protected List<IWFCCell> EntropyQueue = new List<IWFCCell>();

		protected void SortQueue()
		{
			EntropyQueue.Sort();
		}

		public virtual void ClearQueue()
		{
			EntropyQueue = new List<IWFCCell>();
		}

		protected void ShuffleLowestEntropy()
		{
			SortQueue();

			float lowestEntropy = EntropyQueue[0].CalculateEntropy();
			int endIndex;
			for (endIndex = 0; endIndex < EntropyQueue.Count; endIndex++)
			{
				if (EntropyQueue[endIndex].CalculateEntropy() > lowestEntropy)
				{
					break;
				}
			}

			List<IWFCCell> toShuffle = EntropyQueue.GetRange(0, endIndex);

			int n = toShuffle.Count;
			while (n > 1)
			{
				n--;
				// int k = rng.Next(n + 1);
				int k = UnityEngine.Random.Range(0, toShuffle.Count);
				IWFCCell value = toShuffle[k];
				toShuffle[k] = toShuffle[n];
				toShuffle[n] = value;
			}

			EntropyQueue.RemoveRange(0, endIndex);
			EntropyQueue.InsertRange(0, toShuffle);
		}

		public int RemainingCellsToCollapse()
		{
			return EntropyQueue.Count;
		}

		public void Reset()
		{
			EntropyQueue = new List<IWFCCell>();
		}
	}
}