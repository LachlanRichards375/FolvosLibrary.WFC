using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public abstract class IWFCExporter : ScriptableObject
	{
		public abstract void Export();
		public abstract void SetParent(Transform parent);
		public abstract void Reset();
	}
}