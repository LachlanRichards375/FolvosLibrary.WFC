#include "2DGrid.h"

#include <ppl.h>

concurrency::concurrent_queue<concurrency::concurrent_queue<int>> gridLocationsToSetup;
void WFC2DGrid ::Initialize()
{
	for (int x = 0; x < size.x; x++)
	{
		grid[x] = std::vector<WFCCell *>();

		for (int y = 0; y < size.y; y++)
		{
			WFCCell cell = WFCCell(manager, WFCPosition(x, y));
			grid[x][y] = &cell;
			EntropyQueue.insert(cell);
			cell.SetDomain(manager->GetDomain());
		}
	}

	// 	Debug.Log("Getting Entropy Queue");
	// 	for (int x = 0; x < size.x; x++)
	// 	{
	// 		for (int y = 0; y < size.y; y++)
	// 		{
	// 			WFCCell cell = grid[x][y];
	// 			EntropyQueue.Add(cell);
	// 			cell.SetDomain(manager.GetDomain());
	// 			cell.OnCellUpdate += (WFCCellUpdate) = > SortQueue();
	// 		}
	// 	}

	// 	for (int x = 0; x < size.x; x++)
	// 	{
	// 		for (int y = 0; y < size.y; y++)
	// 		{
	// 			// grid[x][y].RuleSetup();
	// 			// Launch the rule setup for each cell
	// 			gridLocationsToSetup.Enqueue(new Vector2Int(x, y));
	// 		}
	// 	}
	// 	tilesToCalc = (int)size.x * (int)size.y;

	// 	Task[] tasks = new Task[manager.MaxThreadCount];
	// 	for (int i = 0; i < manager.MaxThreadCount; i++)
	// 	{
	// 		Task.Run(RuleSetupThread);
	// 	}

	// 	while (tilesToCalc > 0)
	// 	{
	// 		// wait for finish
	// 	}

	// 	continueWorkFlag = false;

	// 	ShuffleLowestEntropy();

	// int tilesToCalc = 0;
	// bool continueWorkFlag = true;
	// ConcurrentQueue<Vector2Int> gridLocationsToSetup = new ConcurrentQueue<Vector2Int>();
	// void RuleSetupThread()
	// {
	// 	while (continueWorkFlag)
	// 	{
	// 		if (gridLocationsToSetup.TryDequeue(out Vector2Int toSetup))
	// 		{
	// 			grid[toSetup.x][toSetup.y].RuleSetup();
	// 			tilesToCalc--;
	// 		}
	// 	}
	// }
}

void WFC2DGrid::RuleSetupThreadWorker()
{
}

WFCCell WFC2DGrid ::GetCell(WFCPosition position)
{
}

bool WFC2DGrid ::HasCollapsed(WFCPosition position)
{
}

bool WFC2DGrid ::PositionInBounds(WFCPosition position)
{
}

void WFC2DGrid ::SortQueue()
{
}

WFCPosition WFC2DGrid ::GetNextCellToCollapse()
{
}

WFCPosition WFC2DGrid ::PopNextCellToCollapse()
{
}

int WFC2DGrid ::RemainingCellsToCollapse()
{
}

void WFC2DGrid ::Reset()
{
}
