using FolvosLibrary.WFC;
using UnityEngine;

public class WFCCellComponent : MonoBehaviour
{
	[SerializeField] public WFCCellStruct CellCaptured;

	public void UpdateVisuals()
	{
	}
}


[System.Serializable]
public struct WFCCellStruct
{
	public WFCTile CollapsedTile;
	public WFCTile[] Domain;
	public string DomainBitMask;

	public WFCCellStruct(WFCTile collapsedTile, WFCTile[] domain, string DomainBitMask)
	{
		CollapsedTile = collapsedTile;
		Domain = domain;
		this.DomainBitMask = DomainBitMask;
	}
}