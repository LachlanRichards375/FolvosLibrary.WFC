namespace FolvosLibrary.WFC
{
	public interface IWFCImporter
	{
		public WFCTile[] Import<Input>(Input input);
	}
}