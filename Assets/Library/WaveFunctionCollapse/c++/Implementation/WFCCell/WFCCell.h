#include "WFCCellDomain.h"
#include "WFCCellUpdate.h"

class WFCCell
{
protected:
	IWFCManager manager;
	WFCPosition position;
	/* data */
public:
	WFCCell(IWFCManager m, WFCPosition p);
	WFCCell(WFCCell &other);
	~WFCCell();
	WFCCellDomain Domain;
	unsigned long CollapsedTile;
	// event Action<WFCCellUpdate> OnCellUpdate;

	// Methods
	//  void SetDomain(List<unsigned long> newDomain);
	void RuleSetup();
	float CalculateEntropy();
	WFCCellUpdate Collapse();
	WFCCellUpdate Collapse(unsigned long toCollapseTo);
	// WFCCellUpdate? DomainCheck(WFCCellUpdate update);
	WFCPosition GetPosition();

	// Comparison operators
	//  int CompareTo(object obj);
	//  int Compare(object x, object y);
	//  override String ToString();
};

WFCCell::WFCCell(IWFCManager m, WFCPosition p)
{
	manager = m;
	position = p;
}

WFCCell::WFCCell(WFCCell &other)
{
	manager = other.manager;
	position = other.position;
}

WFCCell::~WFCCell()
{
}

/*



*/