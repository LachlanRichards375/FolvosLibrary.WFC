using System;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/Manager/2DManager")]
public class WFCManager_2D : ScriptableObject, IWFCManager
{
	IWFCExporter exporter;
	IWFCImporter importer;

	IWFCCell[][] grid = new IWFCCell[0][];
	[SerializeField] WFCTile[] tiles;
	Vector2Int size = new Vector2Int(0, 0);

	SortedList<Vector2Int, float> EntropyQueue = new SortedList<Vector2Int, float>();

	public event Action OnResult;
	public event Action<WFCError> OnError;
	public event Action OnInitialize;

	public event Action OnCleanup;

	public WFCError? Collapse()
	{
		Vector2Int nextTile = EntropyQueue.Keys[0];
		if (EntropyQueue[nextTile] <= 0)
		{
			//We have an issue
			Debug.LogError("Entropy queue.next is < 0");
			return grid[nextTile.x][nextTile.y].GetError();
		}
		grid[nextTile.x][nextTile.y].Collapse();
		EntropyQueue.RemoveAt(0);
		return null;
	}

	public void SetSize(Vector2Int newSize)
	{
		if (newSize.x < 0 || newSize.y < 0)
		{
			return;
		}

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
				if (grid.Length == 0 || y >= grid[0].Length || grid[x] == null || grid[x][y] == null)
				{
					newGrid[x][y] = new WFCCell_2D(this);
				}
				else
				{
					newGrid[x][y] = grid[x][y];
				}
			}
		}

		grid = newGrid;
		size = newSize;
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
		OnInitialize?.Invoke();
	}

	public void Generate()
	{
		GetEntropyQueue();
		for (int i = 0; i < EntropyQueue.Count; i++)
		{
			WFCError? error = Collapse();
			if (error == null)
			{
				//Handle error

			}
		}
	}

	public void Cleanup()
	{
		OnCleanup?.Invoke();
	}

	public WFCTile[] GetDomain()
	{
		Debug.LogError("Get Domain not yet implemented");
		return null;
	}

	public bool HasInitialized()
	{
		return tiles == null || tiles.Length == 0;
	}

	public void DrawSize()
	{
		SetSize(UnityEditor.EditorGUILayout.Vector2IntField("Map Size", size));
	}

	public IWFCCell[][] GetCells()
	{
		return grid;
	}

	public IWFCCell GetCell(Vector2Int position)
	{
		if (position.x < 0 || position.y < 0)
		{
			return null;
		}

		if (position.x >= grid.Length && position.y >= grid[0].Length)
		{
			return null;
		}

		return grid[position.x][position.y];
	}
}