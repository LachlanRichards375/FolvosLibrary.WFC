using System;
using System.Collections.Generic;
using FolvosLibrary.Logging;
using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/Manager/2DManager"), System.Serializable]
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
		WFCCell_2D cell = EntropyQueue[0] as WFCCell_2D;
		Vector2Int nextTile = cell.Position;

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

			for (int y = 0; y < newSize.y; y++)
			{
				newGrid[x][y] = new WFCCell_2D(this, new Vector2Int(x, y));
			}
		}

		grid = newGrid;
		size = newSize;
	}

	void LoadGrid()
	{
		Debug.Log("Getting Entropy Queue");
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				IWFCCell tile = grid[x][y];
				EntropyQueue.Add(tile);
				tile.Domain = new List<WFCTile>(GetDomain());
				tile.OnCellUpdate += OnCellUpdate;
			}
		}

		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				IWFCCell tile = grid[x][y];
				tile.RuleSetup();
			}
		}

		Debug.Log("Pre Shuffle");
		PrintEntropyQueue();

		ShuffleLowestEntropy();

		Debug.Log("Post Shuffle");
		PrintEntropyQueue();
	}

	void SortQueue()
	{
		EntropyQueue.Sort();
	}

	void ShuffleLowestEntropy()
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

		Debug.Log("EntropyQueue Length pre shuffle: " + EntropyQueue.Count);
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
		Debug.Log("EntropyQueue Length post remove: " + EntropyQueue.Count);
		EntropyQueue.InsertRange(0, toShuffle);
		Debug.Log("EntropyQueue Length post re-addition: " + EntropyQueue.Count);
	}

	void OnCellUpdate(WFCCellUpdate update)
	{
		SortQueue();
	}

	public void SetImporter(IWFCImporter importer)
	{
		this.importer = importer;
	}

	public void SetExporter(IWFCExporter exporter)
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

		LoadGrid();

		//On result or error we want to unlock resizing
		OnInitialize?.Invoke();
	}

	public void Generate()
	{
		while (EntropyQueue.Count > 0)
		{
			GenerateOnce();
		}
		OnResult?.Invoke();
	}

	void GenerateOnce()
	{
		Debug.Log("|*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*|");
		WFCError? error = Collapse();
		if (error != null)
		{
			//Handle error
			Debug.LogError("Error occured : " + error.Value.Message);
		}
		//Try print cells after each step.
		//Logger will tell us if not allowed
		PrintCells();
	}

	public void GenerateStep(int step = 1)
	{
		for (int i = 0; i < step && EntropyQueue.Count > 0; i++)
		{
			GenerateOnce();
		}

		if (EntropyQueue.Count > 0)
		{
			((BeachWFCExporter)exporter).Export(GetCells());
		}
		else
		{
			OnResult?.Invoke();
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
		WFCTile[] returner = new WFCTile[tiles.Length];
		tiles.CopyTo(returner, 0);
		return returner;
	}

	public bool HasInitialized()
	{
		return tiles == null || tiles.Length == 0;
	}

	public void DrawSize(bool ForceReset = false)
	{
		Vector2Int newSize = UnityEditor.EditorGUILayout.Vector2IntField("Map Size", size);
		if (ForceReset || newSize != size)
		{
			SetSize(newSize);
		}
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

	public bool HasCollapsed(Vector2Int position)
	{
		if (position.x < 0 || position.y < 0)
		{
			return false;
		}

		if (position.x >= grid.Length || position.y >= grid[0].Length)
		{
			return false;
		}
		Debug.Log($"Checking if {position} has collapsed: hash {grid[position.x][position.y].GetHashCode()}");
		return !(grid[position.x][position.y].CollapsedTile is null);
	}

	public void PrintCells()
	{
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
				toAppend += String.Format("{0,30}", grid[column][row].ToString());
			}
			s += toAppend + "\n";
		}

		message.Message = s;
		Logging.Message(message);

		PrintEntropyQueue();
	}

	public void PrintEntropyQueue()
	{

		Logging.LoggingLevel = Logging.Priority.Low;
		Logging.LoggingGroups = Logging.ProjectGroups.WFCManager;

		Logging.LogMessage message = new Logging.LogMessage();

		message.MessageFrom = Logging.ProjectGroups.WFCManager;
		message.Priority = Logging.Priority.Low;

		string s = $"> Entropy Queue: \n";

		int i = 0;
		foreach (IWFCCell cell in EntropyQueue)
		{
			s += i + ">\t" + cell.GetPosition() + " " + cell.ToString() + "\n";
			i++;
		}


		message.Message = s;
		Logging.Message(message);
	}
}