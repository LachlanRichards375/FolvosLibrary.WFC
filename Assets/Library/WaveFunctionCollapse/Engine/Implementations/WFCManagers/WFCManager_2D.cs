using System;
using System.Collections.Generic;
using FolvosLibrary.Logging;
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

	// SortedList<Vector2Int, IWFCCell> EntropyQueue = new SortedList<Vector2Int, IWFCCell>();
	List<IWFCCell> EntropyQueue = new List<IWFCCell>();

	public event Action OnResult;
	public event Action<WFCError> OnError;
	public event Action OnInitialize;

	public event Action OnCleanup;

	public WFCError? Collapse()
	{
		// Debug.Log($"EntropyQueue.Count: {EntropyQueue.Count}, Cell to collapse: {EntropyQueue[0]}");
		WFCCell_2D cell = EntropyQueue[0] as WFCCell_2D;
		Vector2Int nextTile = cell.Position;

		// Debug.Log($"Cell to collapse at ({nextTile}): {cell}");

		// Debug.Log("Entropy of selected cell is: " + cell.CalculateEntropy());

		if (cell.CalculateEntropy() <= 0f)
		{
			//We have an issue
			Debug.LogError("Entropy queue.next is <= 0");
			return grid[nextTile.x][nextTile.y].GetError();
		}

		cell.Collapse();
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
				if (grid.Length == 0 || x >= grid.Length || y >= grid[0].Length || grid[x] == null || grid[x][y] == null)
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
		Debug.Log("Getting Entropy Queue");
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				WFCCell_2D tile = (WFCCell_2D)grid[x][y];
				EntropyQueue.Add(tile);
				tile.Domain = tiles;
				tile.OnCellUpdate += OnCellUpdate;
				tile.Position = new Vector2Int(x, y);
			}
		}

		Debug.Log($"Got Entropy Queue, Cells filled: {EntropyQueue.Count}, Domain Size: {tiles.Length}");
		Debug.Log($"grid[2][2] Position: {((WFCCell_2D)grid[2][2]).Position}");
		SortQueue();
	}

	void OnCellUpdate(WFCCellUpdate update)
	{
		WFCCell_2D updatedCell = ((WFCCell_2D)update.UpdatedCell);
		int index = EntropyQueue.IndexOf(updatedCell);

		// Debug.Log("Index: " + index);

		if (update.UpdateType == CellUpdateType.Collapsed)
		{
			EntropyQueue.RemoveAt(index);
		}
		else
		{
			EntropyQueue[index] = update.UpdatedCell;
		}

		SortQueue();
	}

	void SortQueue()
	{
		EntropyQueue.Sort();
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
		tiles = importer.Import<string>("https://www.reddit.com/r/196/comments/10nfwvk/boy_likerule/");
		String print = "INITIALIZING \t Domain: ";
		for (int i = 0; i < tiles.Length; i++)
		{
			print += tiles[i].ToString() + ", ";
		}
		Debug.Log(print);
		int yLength = -1;
		if (grid.Length > 0)
		{
			yLength = grid[0].Length;
		}
		Debug.Log("grid size: [" + grid.Length + "," + yLength + "]");

		GetEntropyQueue();

		OnInitialize?.Invoke();
	}

	public void Generate()
	{
		for (int i = 0; i < EntropyQueue.Count; i++)
		{
			WFCError? error = Collapse();
			if (error != null)
			{
				//Handle error

			}
		}
		OnResult?.Invoke();
	}

	public void GenerateStep(int step = 1)
	{
		for (int i = 0; i < step; i++)
		{
			Debug.Log("Collapsing cell");
			WFCError? error = Collapse();
			if (error != null)
			{
				//Handle error
				Debug.LogError("Error occured : " + error.Value.Message);
			}
		}
	}

	public void ClearQueue()
	{
		EntropyQueue = new List<IWFCCell>();
	}

	public void Cleanup()
	{
		OnCleanup?.Invoke();
	}

	public WFCTile[] GetDomain()
	{
		return tiles;
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

		if (position.x >= grid.Length || position.y >= grid[0].Length)
		{
			return null;
		}

		// Debug.Log($"grid.Length: {grid.Length}, grid[0].length: {grid[0].Length}, position: {position}");

		return grid[position.x][position.y];
	}

	public void PrintCells()
	{
		Logging.LoggingLevel = Logging.Priority.Low;
		Logging.LoggingGroups = Logging.ProjectGroups.WFCManager;

		Logging.LogMessage message = new Logging.LogMessage();

		message.MessageFrom = Logging.ProjectGroups.WFCManager;
		message.Priority = Logging.Priority.Low;

		string s = $"WFC Print Cells(): \n" +
		$"\t> Grid size: ({grid.Length},{grid[0].Length}), Entropy Queue.Count: {EntropyQueue.Count}\n";

		//This needs to be set out in this order so it
		//prints to the console the same direction as the exporter output
		for (int row = grid.Length - 1; row >= 0; row--)
		{
			string toAppend = $"{row}\t>";
			for (int column = 0; column < grid[0].Length; column++)
			{
				toAppend += String.Format("{0,15}", grid[column][row].ToString());
			}
			s += toAppend + "\n";
		}
		message.Message = s;

		Logging.Message(message);
	}
}