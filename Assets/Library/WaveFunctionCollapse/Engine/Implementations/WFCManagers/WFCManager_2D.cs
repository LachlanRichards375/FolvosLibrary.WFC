using System;
using FolvosLibrary.Logging;
using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "Folvos/WFC/Manager/2DManager"), System.Serializable]
public class WFCManager_2D : IWFCManager
{

	IWFCCell[][] grid = new IWFCCell[0][];

	public override WFCError? Collapse()
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

	public override WFCError? CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo)
	{
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

		return null;
	}

	public override void SetSize(IWFCPosition size)
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
				newGrid[x][y] = new IWFCCell(this, new IWFCPosition(x, y));
			}
		}

		grid = newGrid;
		this.size = size;
	}

	protected override void LoadGrid()
	{
		Debug.Log("Getting Entropy Queue");
		for (int x = 0; x < size.x; x++)
		{
			for (int y = 0; y < size.y; y++)
			{
				IWFCCell cell = grid[x][y];
				EntropyQueue.Add(cell);
				cell.Domain = GetDomain();
				cell.OnCellUpdate += OnCellUpdate;
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

	public override void Initialize()
	{
		// domain = importer.Import<string>("https://www.reddit.com/r/196/comments/10nfwvk/boy_likerule/");

		domain = ImportDomain();

		String print = "INITIALIZING \t Domain: ";
		for (int i = 0; i < domain.Count; i++)
		{
			print += domain[i].ToString() + ", ";
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
		InvokeOnInitialize();
	}

	public override void GenerateStep(int step = 1)
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
			InvokeOnResult();
		}
	}

	public override async System.Threading.Tasks.Task GenerateTimeLapse()
	{
		while (EntropyQueue.Count > 0)
		{
			Debug.Log("Generating once from Timelapse");
			GenerateOnce();
			((BeachWFCExporter)exporter).Export(GetCells());
			await System.Threading.Tasks.Task.Delay(1000);
		}
		InvokeOnResult();
	}

	public override void DrawSize(bool ForceReset = false)
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

	public override IWFCCell GetCell(IWFCPosition position)
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

	public override bool HasCollapsed(IWFCPosition position)
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

	public override void PrintCells()
	{
		Logging.LogMessage message = new Logging.LogMessage();

		message.MessageFrom = Logging.ProjectGroups.WFCManager;
		message.Priority = Logging.Priority.Low;

		if (Logging.CanLog(message))
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
			Logging.Message(message);

			PrintEntropyQueue();
		}
	}
}