
using UnityEngine;
namespace FolvosLibrary.WFC
{
	public abstract partial class IWFCManager : ScriptableObject
	{
		//EditorWindow
		public abstract void DrawSize(bool ForceReset = false);
	}
}