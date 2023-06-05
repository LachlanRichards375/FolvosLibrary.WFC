
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		protected IWFCExporter exporter;
		protected IWFCImporter importer;
		[SerializeField] protected WFCTile[] domain;

		public abstract WFCError? Collapse();
		public abstract void Initialize();

		public abstract void SetSize(IWFCPosition newSize);
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

		#region One Line Functions
		public virtual void SetImporter(IWFCImporter importer)
		{
			this.importer = importer;
		}

		public virtual void SetExporter(IWFCExporter exporter)
		{
			this.exporter = exporter;
		}

		public virtual bool HasInitialized()
		{
			return domain == null || domain.Length == 0;
		}

		public virtual WFCTile[] GetDomain()
		{
			WFCTile[] returner = new WFCTile[domain.Length];
			domain.CopyTo(returner, 0);
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