
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


			Debug.LogWarning("Force Collapsing first cell to sand");
			WFCTile collapseTo = null;
			foreach (WFCTile tile in domain)
			{
				if (tile.Name == "Sand")
				{
					collapseTo = tile;
					break;
				}
			}

			CollapseSpecificCell(grid.PopNextCellToCollapse(), collapseTo);

			InvokeOnInitialize();
		}

		protected void GenerateOnce()
		{
			PrintGenerationDivider();

			Collapse();
			//Try print cells after each step.
			//Logger will tell us if not allowed
			this.PrintCells();
		}

		protected void Collapse()
		{
			IWFCPosition toCollapse = grid.PopNextCellToCollapse();
			collapseMethod.Collapse(toCollapse);
		}

		protected void CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo)
		{
			collapseMethod.CollapseSpecificCell(position, toCollapseTo);
		}

		//Has to stay until we fix exporter.
		public abstract void UpdateOutput();

		#region One Line Functions
		public void SetImporter(IWFCImporter importer)
		{
			this.importer = importer;
		}

		public IWFCImporter GetImporter() { return importer; }

		public void SetExporter(IWFCExporter exporter)
		{
			this.exporter = exporter;
		}

		public IWFCExporter GetExporter() { return exporter; }

		public void SetGrid(IWFCGrid grid)
		{
			this.grid = grid;
		}

		public void SetCollapseMethod(IWFCCollapseMethod newMethod)
		{
			this.collapseMethod = newMethod;
		}

		#region GridFunctions
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
		public bool HasCollapsed(IWFCPosition position)
		{
			return grid.HasCollapsed(position);
		}
		#endregion

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
		#endregion
	}
}