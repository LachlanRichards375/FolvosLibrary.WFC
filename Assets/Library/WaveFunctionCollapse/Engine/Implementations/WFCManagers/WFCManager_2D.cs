using System;
using System.Threading;
using FolvosLibrary.Logging;
using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "Folvos/WFC/Manager/2DManager"), System.Serializable]
public class WFCManager_2D : IWFCManager
{
	//Has to stay until we fix exporter
	public override void UpdateOutput()
	{
		((BeachWFCExporter)exporter).Export(grid.GetCells());
	}
}