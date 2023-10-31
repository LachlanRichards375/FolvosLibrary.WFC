
using System;
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
				print += domain[i].ToString();
				if (i < domain.Count - 1)
				{
					print += ", ";
				}
			}
			Debug.Log(print);

			grid.Initialize();

			ForceCollapseFirstCellToSand();

			InvokeOnInitialize();
		}

		void ForceCollapseFirstCellToSand()
		{
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
			UpdateOutput();
		}

		protected void GenerateOnce()
		{
			PrintGenerationDivider();

			//Ensure grid is sorted before we collapse the next cell
			grid.SortQueue();

			Collapse();
			//Try print cells after each step.
			//Logger will tell us if not allowed
			this.PrintCells();
		}

		protected void Collapse()
		{
			collapseMethod.Collapse(grid.PopNextCellToCollapse());
		}

		protected void CollapseSpecificCell(WFCPosition position, WFCTile toCollapseTo)
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
			collapseMethod.Reset();
		}
		public void SetSize(WFCPosition newSize)
		{
			grid.SetSize(newSize);
		}
		public WFCPosition GetSize()
		{
			return grid.GetSize();
		}
		public WFCCell GetCell(WFCPosition position)
		{
			return grid.GetCell(position);
		}
		public bool HasCollapsed(WFCPosition position)
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

		public virtual IWFCCollapseMethod GetCollapseMethod()
		{
			return collapseMethod;
		}
		#endregion
	}
}