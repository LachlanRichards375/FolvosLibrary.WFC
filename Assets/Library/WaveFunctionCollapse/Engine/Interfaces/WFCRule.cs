using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class WFCRule
	{
		public WFCRule(){
			
		}

		public WFCRule(WFCRule rule)
		{
		}

		public abstract void DrawRuleProperties();
		public abstract bool Test();
		public abstract void RuleInitialize(IWFCManager manager, IWFCCell Cell);

		public event Action<WFCRule> OnRuleFail;
		protected IWFCCell OwnerCell;

		protected virtual void InvokeOnRuleFail()
		{
			OnRuleFail?.Invoke(this);
		}
	}
}