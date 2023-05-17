using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	public interface IWFCExporter
	{
		public Output Export<Input, Output>(Input input);
		public bool SaveWFCRule(ScriptableObject toSave);
	}
}