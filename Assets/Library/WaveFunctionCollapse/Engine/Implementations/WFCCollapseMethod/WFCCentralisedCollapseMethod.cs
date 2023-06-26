using System.Collections.Generic;

namespace FolvosLibrary.WFC
{
	public class WFCCentralisedCollapseMethod : IWFCCollapseMethod
	{
		Dictionary<IWFCPosition, List<IWFCCell>> toAlert = new Dictionary<IWFCPosition, List<IWFCCell>>();

		IWFCManager manager;

		public WFCCentralisedCollapseMethod(IWFCManager manager)
		{
			this.manager = manager;
		}


		public void Collapse(IWFCPosition position)
		{
			IWFCCell toCollapse = manager.GetCell(position);

			if (toCollapse != null)
			{
				toCollapse.Collapse();
			}
		}

		public void RegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toRegister)
		{
			if (!toAlert.ContainsKey(positionOfInterest))
			{
				toAlert.Add(positionOfInterest, new List<IWFCCell>());
			}

			List<IWFCCell> addTo = toAlert[positionOfInterest];

			if (addTo.Contains(toRegister))
			{
				return;
			}

			addTo.Add(toRegister);
		}

		public void DeRegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toDeregister)
		{
			if (!toAlert.ContainsKey(positionOfInterest))
			{
				return;
			}

			toAlert[positionOfInterest].Remove(toDeregister);
		}
	}
}