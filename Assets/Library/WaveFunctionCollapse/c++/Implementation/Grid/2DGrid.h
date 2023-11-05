#include "IWFCGrid.h"

class WFC2DGrid : public IWFCGrid
{
protected:
	std::vector<std::vector<WFCCell>> grid;

public:
	using IWFCGrid::IWFCGrid;
	void SetSize(WFCPosition size);
	WFCPosition GetSize();
	void Initialize();
	WFCCell GetCell(WFCPosition position);
	bool HasCollapsed(WFCPosition position);
	bool PositionInBounds(WFCPosition position);
	void SortQueue();
	WFCPosition GetNextCellToCollapse();
	WFCPosition PopNextCellToCollapse();
	int RemainingCellsToCollapse();
	void Reset();
};
