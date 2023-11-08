class IWFCCollapseMethod;
class IWFCGrid;

#include "IWFCGrid.h"
#include "IWFCCollapseMethod.h"

class IWFCManager
{
private:
	/* data */
	void ForceCollapseFirstCellToSand();
	int maximumThreadCount = 1;

protected:
	IWFCGrid &grid;
	IWFCCollapseMethod &collapseMethod;
	// List<WFCTile> domain;
	void GenerateOnce();

	void Collapse();

	void CollapseSpecificCell(WFCPosition position, unsigned long toCollapseTo);

public:
	IWFCManager(/* args */);
	~IWFCManager();
	void Initialize();
	virtual void UpdateOutput() = 0;
	void SetGrid(IWFCGrid grid);
	void SetCollapseMethod(IWFCCollapseMethod newMethod);
	void Reset();
	void SetSize(WFCPosition newSize);
	WFCPosition GetSize();
	WFCCell GetCell(WFCPosition position);
	bool HasCollapsed(WFCPosition position);
	bool HasInitialized();
	std::vector<unsigned long> GetDomain();
	void Cleanup();
	IWFCCollapseMethod GetCollapseMethod();
	const int GetMaxThreadCount();
	void SetMaxThreadCount(const int threadCount);
};

IWFCManager::IWFCManager(/* args */)
{
}

IWFCManager::~IWFCManager()
{
}
