
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		protected IWFCExporter exporter;
		protected IWFCImporter importer;
		protected IWFCCollapseMethod collapseMethod;
		protected IWFCGrid grid;
		protected List<WFCTile> domain;

		public void SetGrid(IWFCGrid grid)
		{
			this.grid = grid;
		}

		public WFCError? Collapse()
		{
			return grid.Collapse();
		}

		public WFCError? CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo)
		{
			return grid.CollapseSpecificCell(position, toCollapseTo);
		}

		public void Initialize()
		{
			domain = new List<WFCTile>(importer.Import<string>("a"));


			string print = "INITIALIZING \t Domain: ";
			for (int i = 0; i < domain.Count; i++)
			{
				print += domain[i].ToString() + ", ";
			}
			Debug.Log(print);

			grid.Initialize();
			InvokeOnInitialize();
		}

		public void Reset()
		{
			grid.Reset();
		}

		public void SetSize(IWFCPosition newSize)
		{
			grid.SetSize(newSize);
		}
		public IWFCPosition GetSize()
		{
			return grid.GetSize();
		}
		public IWFCCell GetCell(IWFCPosition position)
		{
			return grid.GetCell(position);
		}
		//dynamic because it can return different results
		public dynamic GetCells()
		{
			return grid.GetCells();
		}
		public bool HasCollapsed(IWFCPosition position)
		{
			return grid.HasCollapsed(position);
		}

		//Generation
		public virtual void Generate()
		{
			while (grid.RemainingCellsToCollapse() > 0)
			{
				GenerateOnce();
			}
			InvokeOnResult();
		}
		public virtual void GenerateStep(int step = 1)
		{
			for (int i = 0; i < step && grid.RemainingCellsToCollapse() > 0; i++)
			{
				GenerateOnce();
			}

			if (grid.RemainingCellsToCollapse() > 0)
			{
				UpdateOutput();
			}

			if (grid.RemainingCellsToCollapse() <= 0)
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
			while (grid.RemainingCellsToCollapse() > 0)
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
			if (grid.RemainingCellsToCollapse() > 0)
			{

			}
			InvokeOnResult();
		}

		//Has to stay until we fix exporter.
		public abstract void UpdateOutput();

		#region One Line Functions
		public virtual void SetImporter(IWFCImporter importer)
		{
			this.importer = importer;
		}

		public IWFCImporter GetImporter() { return importer; }

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

		public virtual bool DrawSize(bool ForceReset = false)
		{
			if (grid == null)
			{
				return false;
			}
			grid.DrawSize(ForceReset);
			return true;
		}
		#endregion
	}
}