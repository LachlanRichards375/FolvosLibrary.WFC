using System;
using System.Collections.Generic;
using FolvosLibrary.Logging;
using UnityEngine;
using System.Threading.Tasks;

namespace FolvosLibrary.WFC
{
	[CreateAssetMenu(menuName = "Folvos/WFC/Grids/2DGrid"), System.Serializable]
	public class WFCGrid2D : IWFCGrid
	{
		WFCPosition size;
		WFCCell[][] grid = new WFCCell[0][];

		public override void SetSize(WFCPosition size)
		{
			Vector2Int newSize = size.AsVector2Int();
			if (newSize.x < 0 || newSize.y < 0)
			{
				return;
			}
			this.size = size;
		}

		public override WFCPosition GetSize()
		{
			if (grid == null || grid[0] == null)
			{
				return new WFCPosition(0, 0);
			}

			return new WFCPosition(grid.Length, grid[0].Length);
		}

		public override void Initialize()
		{
			InitializeGrid();

			Debug.Log($"grid size: {size}");

			LoadGrid();
		}

		protected void InitializeGrid()
		{
			Vector2Int newGridSize = size.AsVector2Int();
			WFCCell[][] newGrid = new WFCCell[newGridSize.x][];
			for (int x = 0; x < newGridSize.x; x++)
			{
				newGrid[x] = new WFCCell[newGridSize.y];

				for (int y = 0; y < newGridSize.y; y++)
				{
					newGrid[x][y] = new WFCCell(manager, new WFCPosition(x, y));
				}
			}

			grid = newGrid;
		}

		protected void LoadGrid()
		{
			Debug.Log("Getting Entropy Queue");
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					WFCCell cell = grid[x][y];
					EntropyQueue.Add(cell);
					cell.SetDomain(manager.GetDomain());
					cell.OnCellUpdate += (WFCCellUpdate) => SortQueue();
				}
			}

			Task[] tasks = new Task[(int)size.x * (int)size.y];
			int taskIndex = 0;
			// Debug.Log("Task List Size: " + tasks.Length);
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					//grid[x][y].RuleSetup();
					//Launch the rule setup for each cell
					// Debug.Log($"taskIndex ({taskIndex}, on ({x},{y}))");
					int localX = x;
					int localY = y;
					tasks[taskIndex] = Task.Run(() => grid[localX][localY].RuleSetup());
					taskIndex++;
				}
			}

			//Wait for all jobs to be finished
			Task.WaitAll(tasks);

			ShuffleLowestEntropy();
		}

		public override bool HasCollapsed(WFCPosition position)
		{
			if (!PositionInBounds(position))
			{
				return false;
			}

			Vector2Int vec = position.AsVector2Int();
			return !(grid[vec.x][vec.y].CollapsedTile is null);
		}

		public override bool PositionInBounds(WFCPosition position)
		{
			Vector2Int pos = position.AsVector2Int();

			if (pos.x < 0 || pos.x >= grid.Length)
			{
				return false;
			}

			if (pos.y < 0 || pos.y >= grid[0].Length)
			{
				return false;
			}

			return true;
		}

		public override WFCCell GetCell(WFCPosition position)
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

		public WFCCell[][] GetCells()
		{
			return grid;
		}

		public override void DrawSize(bool ForceReset = false)
		{
			Vector2Int newSize = UnityEditor.EditorGUILayout.Vector2IntField("Map Size", size.AsVector2Int());
			if (ForceReset || newSize != size.AsVector2Int())
			{
				SetSize(new WFCPosition(newSize));
			}
		}

		public override void PrintCells()
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
	}
}