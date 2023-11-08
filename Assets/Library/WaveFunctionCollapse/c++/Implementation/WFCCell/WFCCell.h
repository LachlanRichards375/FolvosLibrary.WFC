#include "WFCCellDomain.h"
#include "WFCCellUpdate.h"
#include "IWFCManager.h"

class WFCCell
{
protected:
	IWFCManager *manager;
	WFCPosition position;
	/* data */
public:
	WFCCell(IWFCManager *m, WFCPosition p);
	WFCCell(WFCCell &other);
	~WFCCell();
	WFCCellDomain Domain;
	unsigned long CollapsedTile;
	// event Action<WFCCellUpdate> OnCellUpdate;

	// Methods
	void SetDomain(std::vector<unsigned long> newDomain);
	void RuleSetup() const;
	float CalculateEntropy() const;
	WFCCellUpdate Collapse();
	WFCCellUpdate Collapse(unsigned long toCollapseTo);
	// WFCCellUpdate? DomainCheck(WFCCellUpdate update);
	WFCPosition GetPosition() const;

	bool operator<(const WFCCell &other) const
	{
		return (this->CalculateEntropy() < other.CalculateEntropy());
	}
};

WFCCell::WFCCell(IWFCManager *m, WFCPosition p)
{
	WFCCell::manager = m;
	WFCCell::position = p;
}

WFCCell::WFCCell(WFCCell &other)
{
	WFCCell::manager = other.manager;
	WFCCell::position = other.position;
}

WFCCell::~WFCCell()
{
}

/*



*/