using FolvosLibrary.WFC;
using UnityEngine;

[CreateAssetMenu(menuName = "WFC/Importer/BeachImporter"), System.Serializable]
class BeachImporter : ScriptableObject, IWFCImporter
{
	[SerializeField] WFCTile[] returner;
	public WFCTile[] Import<Input>(Input input)
	{
		return returner;
	}

	public void Reset()
	{

	}
}