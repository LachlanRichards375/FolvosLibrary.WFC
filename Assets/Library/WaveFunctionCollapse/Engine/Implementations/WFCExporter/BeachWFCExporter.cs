using System.Collections;
using System.Collections.Generic;
using FolvosLibrary.WFC;
using UnityEngine;


[CreateAssetMenu(menuName = "WFC/Exporter/BeachExporter")]
public class BeachWFCExporter : ScriptableObject, IWFCExporter
{
	public void Export(){
		Debug.LogError("Export should not be called, use Export(IWFCCell[][]) instead");
	}
	public GameObject[][] Export(IWFCCell[][] input)
	{
		IWFCCell[][] toExport;
		toExport = input as IWFCCell[][];

		GameObject[][] Exported = new GameObject[toExport.Length][];

		for(int x = 0; x < toExport.Length; x++){
			Exported[x] = new GameObject[toExport[0].Length];
			for(int y = 0; y < toExport[0].Length; y++){
				Exported[x][y] = new GameObject($"{toExport[x][y].ToString()}{{{x},{y}}}");
			}
		}

		return Exported;
	}
}
