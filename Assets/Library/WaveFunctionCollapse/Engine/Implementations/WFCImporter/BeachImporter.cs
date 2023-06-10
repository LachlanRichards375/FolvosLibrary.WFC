using FolvosLibrary.WFC;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Folvos/WFC/Importer/BeachImporter"), System.Serializable]
public class BeachImporter : IWFCImporter
{
	public WFCTile[] returner;
	public override WFCTile[] Import<Input>(Input input)
	{
		List<WFCTile> toReturn = new List<WFCTile>();
		for (int i = 0; i < returner.Length; i++)
		{
			toReturn.Add(WFCTile.CreateTile(returner[i].TileData, returner[i].Rules));
		}
		return toReturn.ToArray();
	}

	public override void Reset()
	{

	}
}