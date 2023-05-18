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
		public abstract void DrawRuleProperties();
		public abstract bool Test();
		public event Action OnRuleFail;

		public abstract void RuleInitialize(IWFCManager manager, IWFCCell Cell);
	}
}