namespace FolvosLibrary.WFC
{
	public interface IWFCGrid
	{
		public void SetManager(IWFCManager manager);

		public WFCError? Collapse();
		public WFCError? CollapseSpecificCell(IWFCPosition position, WFCTile toCollapseTo);
		public void SetSize(IWFCPosition size);
		public IWFCPosition GetSize();
		public void Initialize();
		public void DrawSize(bool ForceReset = false);
		public IWFCCell[][] GetCells();
		public IWFCCell GetCell(IWFCPosition position);
		public bool HasCollapsed(IWFCPosition position);
		public void PrintCells();

		public int RemainingCellsToCollapse();
		public void Reset();

	}
}