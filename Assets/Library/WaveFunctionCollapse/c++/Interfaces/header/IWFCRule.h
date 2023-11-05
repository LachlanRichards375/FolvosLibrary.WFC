#include "IWFCManager.h"
class IWFCRule
{
private:
	/* data */
protected:
	IWFCManager manager;
	// void InvokeRuleActivated(WFCCellUpdate update)
	//  void InvokeOnRuleFail()
public:
	IWFCRule(/* args */);
	IWFCRule(WFCRule rule);
	~IWFCRule();
	virtual bool Test(WFCCellUpdate? cellUpdate, WFCCell ownerCell) = 0;
	virtual void RuleInitialize(IWFCManager manager, Vector2Int CellPos) = 0;
	// Rule activates when something 'trigers' this tile to check it's domain
	// event Action<WFCCellUpdate> OnRuleActivated;
	// event Action<WFCRule> OnRuleFail;
};

IWFCRule::IWFCRule(/* args */)
{
}

IWFCRule::~IWFCRule()
{
}

IWFCRule::IWFCRule(WFCRule rule)
{
	IWFCRule::manager = rule.manager
}