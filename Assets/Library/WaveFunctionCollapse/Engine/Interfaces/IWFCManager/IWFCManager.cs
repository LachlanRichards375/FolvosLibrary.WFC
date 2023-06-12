
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		protected IWFCExporter exporter;
		protected IWFCImporter importer;
		[SerializeField] protected List<WFCTile> domain;

		public abstract WFCError? Collapse();
		public abstract WFCError? CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo);
		public abstract void Initialize();

		protected IWFCPosition size;
		public abstract void SetSize(IWFCPosition newSize);
		public IWFCPosition GetSize()
		{
			return size;
		}
		protected abstract void LoadGrid();
		public abstract IWFCCell GetCell(IWFCPosition position);
		public abstract bool HasCollapsed(IWFCPosition position);

		//Generation
		public virtual void Generate()
		{
			while (EntropyQueue.Count > 0)
			{
				GenerateOnce();
			}
			InvokeOnResult();
		}
		public virtual void GenerateStep(int step = 1)
		{
			for (int i = 0; i < step && EntropyQueue.Count > 0; i++)
			{
				GenerateOnce();
			}

			if (EntropyQueue.Count > 0)
			{
				UpdateOutput();
			}

			if (EntropyQueue.Count <= 0)
			{
				InvokeOnResult();
			}
		}
		protected void GenerateOnce()
		{
			PrintGenerationDivider();

			WFCError? error = Collapse();
			if (error != null)
			{
				//Handle error
				Debug.LogError("Error occured : " + error.Value.Message);
			}
			//Try print cells after each step.
			//Logger will tell us if not allowed
			this.PrintCells();
		}
		public virtual async System.Threading.Tasks.Task GenerateTimeLapse(System.Threading.CancellationTokenSource cancellationToken, int millsBetweenStep)
		{
			while (EntropyQueue.Count > 0)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				Debug.Log("Generating once from Timelapse");
				GenerateOnce();
				UpdateOutput();
				await System.Threading.Tasks.Task.Delay(millsBetweenStep);
			}
			if (EntropyQueue.Count > 0)
			{

			}
			InvokeOnResult();
		}

		public abstract void UpdateOutput();

		#region One Line Functions
		public virtual void SetImporter(IWFCImporter importer)
		{
			this.importer = importer;
		}

		public IWFCImporter GetImporter() { return importer; }


		protected List<WFCTile> ImportDomain()
		{
			if (importer == null)
			{
				return null;
			}

			List<WFCTile> returner = new List<WFCTile>();
			foreach (WFCTile tile in importer.Import<string>("You like kissing boys don't you?"))
			{
				returner.Add(WFCTile.CreateTile(tile));
			}

			return returner;
		}


		public virtual void SetExporter(IWFCExporter exporter)
		{
			this.exporter = exporter;
		}

		public IWFCExporter GetExporter() { return exporter; }

		public virtual bool HasInitialized()
		{
			return domain == null || domain.Count == 0;
		}

		public virtual List<WFCTile> GetDomain()
		{
			List<WFCTile> returner = new List<WFCTile>();
			foreach (WFCTile tile in domain)
			{
				returner.Add(WFCTile.CreateTile(tile));
			}
			return returner;
		}

		public virtual void Cleanup()
		{
			InvokeOnCleanup();
		}

		protected void OnCellUpdate(WFCCellUpdate update)
		{
			SortQueue();
		}
		#endregion

		#region Entropy Queue
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
		#endregion
	}
}