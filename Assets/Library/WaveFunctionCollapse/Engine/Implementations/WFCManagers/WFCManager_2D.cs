using System;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public class WFCManager_2D : IWFCManager
	{
		IWFCExporter exporter;
		IWFCImporter importer;

		IWFCCell[][] grid;
		WFCTile[] tiles;
		Vector2Int size;

		SortedList<Vector2Int, float> EntropyQueue = new SortedList<Vector2Int, float>();

		public event Action OnResult;
		public event Action<WFCError> OnError;
		public event Action OnInitialize;
		public event Action OnCleanup;

		public void Collapse()
		{
			GetEntropyQueue();
			for (int i = 0; i < EntropyQueue.Count; i++)
			{
				Vector2Int nextTile = EntropyQueue.Keys[0];
				if (EntropyQueue[nextTile] <= 0)
				{
					//We have an issue
					Debug.LogError("Entropy queue.next is < 0");
				}
				grid[nextTile.x][nextTile.y].Collapse();
			}
		}

		public void SetSize(Vector2Int newSize)
		{
			IWFCCell[][] newGrid = new IWFCCell[newSize.x][];
			for (int x = 0; x < newSize.x; x++)
			{
				newGrid[x] = new IWFCCell[newSize.y];

				//If outside bounds of old grid we can skip
				if (x > grid.Length)
				{
					continue;
				}
				for (int y = 0; y < newSize.y; y++)
				{
					//If outside bounds of old grid we can skip
					if (y > grid[0].Length)
					{
						break;
					}
					newGrid[x][y] = grid[x][y];
				}
			}

			grid = newGrid;
		}

		void GetEntropyQueue()
		{
			for (int x = 0; x < size.x; x++)
			{
				for (int y = 0; y < size.y; y++)
				{
					WFCCell_2D tile = (WFCCell_2D)grid[x][y];
					EntropyQueue.Add(new Vector2Int(x, y), tile.CalculateEntropy());
					tile.Domain = tiles;
					tile.OnCellUpdate += OnCellUpdate;
					tile.Position = new Vector2Int(x, y);
				}
			}
		}

		void OnCellUpdate(WFCCellUpdate update)
		{
			Vector2Int position = ((WFCCell_2D)update.UpdatedCell).Position;
			if (update.UpdateType == CellUpdateType.Collapsed)
			{
				EntropyQueue.Remove(position);
			}
			else
			{
				EntropyQueue[position] = update.UpdatedCell.CalculateEntropy();
			}
		}

		public void SetImporter(IWFCImporter importer)
		{
			this.importer = importer;
		}

		void IWFCManager.SetExporter(IWFCExporter exporter)
		{
			this.exporter = exporter;
		}

		public void Initialize()
		{
		}

		public void Generate()
		{
		}

		public void Cleanup()
		{
		}

		public WFCTile[] GetDomain()
		{
		}
	}
}