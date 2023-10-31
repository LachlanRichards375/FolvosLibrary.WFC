using System;
using UnityEngine;
using UnityEditor;

namespace FolvosLibrary.WFC
{

	[CreateAssetMenu(menuName = "Folvos/WFC/Tile/TileList", order = 1), System.Serializable]
	public class WFCTileList : ScriptableObject
	{
		[SerializeReference]
		private WFCTile[] tiles;

		public bool ValidateTileBitmasks()
		{
			if (tiles.Length > 128)
			{
				Debug.LogError("More than 128 tiles in this tile list", this);
				return false;
			}

			ulong ClaimedBitMaskID = 1;
			for (int i = 0; i < tiles.Length; i++)
			{
				tiles[i].ID = ClaimedBitMaskID;
				ClaimedBitMaskID <<= 1;
			}

			return true;
		}
	}


}