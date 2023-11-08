#pragma once
#include <set>
#include <vector>
#include "WFCPosition.h"
#include "WFCCell.h"
#include "IWFCManager.h"

class IWFCGrid
{
protected:
	IWFCManager *manager;
	// List<WFCCell> EntropyQueue = new List<WFCCell>();
	std::set<WFCCell> EntropyQueue = std::set<WFCCell>();
	WFCPosition size;

	void
	ShuffleLowestEntropy()
	{
	}

public:
	IWFCGrid(IWFCManager *manager, WFCPosition size);
	~IWFCGrid();

	WFCPosition GetSize() { return size; }

	virtual void Initialize() = 0;
	virtual WFCCell GetCell(WFCPosition position) = 0;
	virtual bool HasCollapsed(WFCPosition position) = 0;
	virtual bool PositionInBounds(WFCPosition position) = 0;
	// Do we need to sort queue?
	virtual void SortQueue() = 0;
	virtual WFCPosition GetNextCellToCollapse() = 0;
	virtual WFCPosition PopNextCellToCollapse() = 0;
	virtual int RemainingCellsToCollapse() = 0;
	virtual void Reset() = 0;
};

IWFCGrid::IWFCGrid(IWFCManager *manager, WFCPosition size)
{
	IWFCGrid::manager = manager;
	IWFCGrid::size = size;
}

IWFCGrid::~IWFCGrid()
{
}

/*
public void SortQueue()
{
	EntropyQueue.Sort();
}

public virtual void ClearQueue()
{
	EntropyQueue = new List<WFCCell>();
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

	List<WFCCell> toShuffle = EntropyQueue.GetRange(0, endIndex);

	int n = toShuffle.Count;
	while (n > 1)
	{
		n--;
		// int k = rng.Next(n + 1);
		int k = UnityEngine.Random.Range(0, toShuffle.Count);
		WFCCell value = toShuffle[k];
		toShuffle[k] = toShuffle[n];
		toShuffle[n] = value;
	}

	EntropyQueue.RemoveRange(0, endIndex);
	EntropyQueue.InsertRange(0, toShuffle);
}

public WFCPosition GetNextCellToCollapse()
{
	return EntropyQueue[0].GetPosition();
}

public WFCPosition PopNextCellToCollapse()
{
	WFCPosition returner = EntropyQueue[0].GetPosition();
	EntropyQueue.RemoveAt(0);
	return returner;
}

public int RemainingCellsToCollapse()
{
	return EntropyQueue.Count;
}

public void PrintEntropyQueue()
{
	Logging.Logging.LogMessage message = new Logging.Logging.LogMessage();

	message.MessageFrom = Logging.Logging.ProjectGroups.WFCManager;
	message.Priority = Logging.Logging.Priority.Low;

	string s = $"> Entropy Queue: \n";

	int i = 0;
	foreach (WFCCell cell in EntropyQueue)
	{
		s += i + ">\t" + cell.GetPosition() + " " + cell.ToString() + "\n";
		i++;
	}

	message.Message = s;
	Logging.Logging.Message(message);
}

public void Reset()
{
	EntropyQueue = new List<WFCCell>();
}
*/
