class IWFCManager;

#include "IWFCManager.h"
#include "WFCPosition.h"
#include "WFCCell.h"

class IWFCCollapseMethod
{
protected:
	IWFCManager &manager;

public:
	IWFCCollapseMethod(/* args */);
	~IWFCCollapseMethod();
	void Initalize(IWFCManager &manager);
	void Collapse(WFCPosition position);
	void CollapseSpecificCell(WFCPosition position, unsigned long collapseTo);
	void RegisterForCellUpdates(WFCPosition positionOfInterest, WFCCell toRegister);
	void DeRegisterForCellUpdates(WFCPosition positionOfInterest, WFCCell toDeregister);
	void Reset();
	void SetManager(IWFCManager manager);
};

IWFCCollapseMethod::IWFCCollapseMethod(/* args */)
{
}

IWFCCollapseMethod::~IWFCCollapseMethod()
{
}
