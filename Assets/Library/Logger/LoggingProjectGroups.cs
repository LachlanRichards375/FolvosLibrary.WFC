namespace FolvosLibrary.Logging
{
	public static partial class Logging
	{
		[System.Flags]
		public enum ProjectGroups
		{
			Nothing = 0,
			Debug = 1,
			Test = 2,
			WFCManager = 4,
		}
	}
}