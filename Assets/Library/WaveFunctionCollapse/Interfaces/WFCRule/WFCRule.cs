using System;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	[System.Serializable]
	public abstract class WFCRule
	{
		public WFCRule()
		{
		}

		public WFCRule(WFCRule rule)
		{
		}

		abstract public void DrawRuleProperties();
	}
}