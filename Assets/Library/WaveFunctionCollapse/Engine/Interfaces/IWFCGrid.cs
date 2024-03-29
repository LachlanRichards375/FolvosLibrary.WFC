using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class IWFCGrid : ScriptableObject
	{
		protected IWFCManager manager;
		public void SetManager(IWFCManager manager)
		{
			this.manager = manager;
		}

		public abstract void SetSize(WFCPosition size);
		public abstract WFCPosition GetSize();
		public abstract void Initialize();
		public abstract void DrawSize(bool ForceReset = false);
		public abstract WFCCell GetCell(WFCPosition position);
		public abstract bool HasCollapsed(WFCPosition position);
		public abstract void PrintCells();

		public abstract bool PositionInBounds(WFCPosition position);

		//Entropy Queue
		protected List<WFCCell> EntropyQueue = new List<WFCCell>();

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

	}
}