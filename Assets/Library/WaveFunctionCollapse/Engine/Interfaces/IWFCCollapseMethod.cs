using System;
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract class IWFCCollapseMethod : ScriptableObject
	{
		public abstract void Collapse(IWFCPosition position);
		public abstract void CollapseSpecificCell(IWFCPosition position, WFCTile collapseTo);
		public abstract void RegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toRegister);
		public abstract void DeRegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toDeregister);

		protected IWFCManager manager;
		public void SetManager(IWFCManager manager)
		{
			this.manager = manager;
		}

		public abstract void DrawOptions();
	}
}