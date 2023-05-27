namespace FolvosLibrary.WFC
{
	public abstract class IWFCImporter : UnityEngine.ScriptableObject
	{
		public abstract WFCTile[] Import<Input>(Input input);
		public abstract void Reset();
	}
}