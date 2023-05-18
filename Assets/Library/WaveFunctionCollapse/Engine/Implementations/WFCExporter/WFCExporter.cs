using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using UnityEngine;


[CreateAssetMenu(menuName = "WFC/Exporter/BeachExporter")]
public class WFCExporter : ScriptableObject, IWFCExporter
{
	public Output Export<Input, Output>(Input input)
	{
		throw new System.NotImplementedException();
	}

	public bool SaveWFCRule(ScriptableObject toSave)
	{
		throw new System.NotImplementedException();
	}
}
