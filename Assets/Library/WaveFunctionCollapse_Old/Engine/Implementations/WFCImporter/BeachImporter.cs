using FolvosLibrary.WFC;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Folvos/WFC/Importer/BeachImporter"), System.Serializable]
public class BeachImporter : IWFCImporter
{
	public WFCTile[] returner;
	public override WFCTile[] Import<Input>(Input input)
	{
		return (WFCTile[])returner.Clone();
	}

	public override void Reset()
	{

	}
}