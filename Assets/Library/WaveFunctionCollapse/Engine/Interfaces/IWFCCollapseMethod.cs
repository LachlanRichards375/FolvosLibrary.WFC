
namespace FolvosLibrary.WFC
{
	public interface IWFCCollapseMethod
	{
		public void Collapse(IWFCPosition position);
		public void RegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toRegister);
		public void DeRegisterForCellUpdates(IWFCPosition positionOfInterest, IWFCCell toDeregister);
	}
}