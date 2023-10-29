using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract class IWFCCollapseMethod : ScriptableObject
	{
		public abstract void Collapse(WFCPosition position);
		public abstract void CollapseSpecificCell(WFCPosition position, WFCTile collapseTo);
		public abstract void RegisterForCellUpdates(WFCPosition positionOfInterest, IWFCCell toRegister);
		public abstract void DeRegisterForCellUpdates(WFCPosition positionOfInterest, IWFCCell toDeregister);

		protected IWFCManager manager;
		public void SetManager(IWFCManager manager)
		{
			this.manager = manager;
		}

		public abstract void DrawOptions();
	}
}