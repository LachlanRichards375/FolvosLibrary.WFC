using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "Folvos/WFC/Importer/BeachImporter"), System.Serializable]
class BeachImporter : IWFCImporter
{
	[SerializeField] WFCTile[] returner;
	public override WFCTile[] Import<Input>(Input input)
	{
		return returner;
	}

	public override void Reset()
	{

	}
}