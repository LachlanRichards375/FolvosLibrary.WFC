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
		public WFCRule()
		{
		}

		public WFCRule(WFCRule rule)
		{
		}

		public virtual void DrawRuleProperties()
		{
			InvalidateTileOnFail = (bool)EditorGUILayout.Toggle("Invalidate tile on rule failure", InvalidateTileOnFail);
		}

		public abstract bool Test();
		public abstract bool Test(WFCCellUpdate? cellUpdate, IWFCCell ownerCell);
		public abstract void RuleInitialize(IWFCManager manager, Vector2Int CellPos);

		public bool InvalidateTileOnFail = false;

		//Rule activates when something 'trigers' this tile to check it's domain
		public event Action<WFCCellUpdate> OnRuleActivated;
		public event Action<WFCRule> OnRuleFail;
		protected IWFCManager manager;

		public abstract string GetTargetCells();

		protected virtual void InvokeRuleActivated(WFCCellUpdate update)
		{
			OnRuleActivated?.Invoke(update);
		}

		protected virtual void InvokeOnRuleFail()
		{
			OnRuleFail?.Invoke(this);
		}
	}
}