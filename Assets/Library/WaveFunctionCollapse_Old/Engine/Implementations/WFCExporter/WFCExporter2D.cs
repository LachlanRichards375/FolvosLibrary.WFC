using System.Collections.Generic;
using FolvosLibrary.WFC;
using UnityEngine;


[CreateAssetMenu(menuName = "Folvos/WFC/OLD/Exporter/2D_Exporter"), System.Serializable]
public class WFCExporter2D : IWFCExporter
{
	Transform parent;
	GameObject[][] Exported = new GameObject[0][];
	public override void Export(ulong[] exportedTiles, WFCPosition size, WFCTileList tileList)
	{
		//Setup the dictionary
		Dictionary<ulong, WFCTile> tileDict = new();
		foreach (WFCTile t in tileList.tiles)
		{
			tileDict.Add(t.ID, t);
		}

		//Do we need to create new gameobjects or can we use old ones?
		bool createGameObjects = Exported.Length != size.x || Exported[0].Length != size.y || Exported[0][0] == null;
		if (createGameObjects)
		{
			while (parent.childCount > 0)
			{
				DestroyImmediate(parent.GetChild(0).gameObject);
			}

			Exported = new GameObject[(int)size.x][];
		}


		//The grid loop 
		for (int x = 0; x < (int)size.x; x++)
		{
			if (createGameObjects)
			{
				Exported[x] = new GameObject[(int)size.y];
			}
			for (int y = 0; y < (int)size.y; y++)
			{
				if (createGameObjects)
				{
					Exported[x][y] = new GameObject($"{{{x},{y}}}", typeof(SpriteRenderer), typeof(WFCCellComponent));
					if (parent != null)
					{
						Exported[x][y].transform.SetParent(parent);
					}
				}

				GameObject target = Exported[x][y];
				Transform t = target.transform;
				t.position = new Vector3(x, y, 0f);
				WFCTile collapsedTile = tileDict[exportedTiles[x * size.AsVector2Int().x + y]];

				SpriteRenderer sR = target.GetComponent<SpriteRenderer>();
				sR.sprite = collapsedTile.TileData.Sprite;
			}
		}
	}

	public override void Reset()
	{
		foreach (GameObject[] row in Exported)
		{
			foreach (GameObject g in row)
			{
				DestroyImmediate(g);
			}
		}
		Exported = new GameObject[0][];
	}

	public override void SetParent(Transform parent)
	{
		this.parent = parent;
	}
}
