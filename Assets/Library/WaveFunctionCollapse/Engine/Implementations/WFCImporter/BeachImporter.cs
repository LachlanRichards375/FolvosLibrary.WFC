using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/Importer/BeachImporter")]
class BeachImporter : ScriptableObject, IWFCImporter
{
	[SerializeField] WFCTile[] returner;
	public WFCTile[] Import<Input>(Input input)
	{
		return returner;
	}
}