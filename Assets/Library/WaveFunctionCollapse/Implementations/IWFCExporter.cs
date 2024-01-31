using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class IWFCExporter : ScriptableObject
	{
		public abstract void Export(ulong[] var, WFCPosition size, WFCTileList tileList);
		public abstract void SetParent(Transform parent);
		public abstract void Reset();
	}
}