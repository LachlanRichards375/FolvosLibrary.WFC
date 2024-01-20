using System.Collections.Generic;
using FolvosLibrary.WFC;
using UnityEngine;


[CreateAssetMenu(menuName = "Folvos/WFC/Exporter/BeachExporter"), System.Serializable]
public class BeachWFCExporter : IWFCExporter
{
	Transform parent;
	GameObject[][] Exported = new GameObject[0][];
	public override void Export(ulong[] var, WFCPosition size, WFCTileList tileList)
	{
		string message = "Exporting: \n";
		WFCCell[][] cells = new WFCCell[(int)size.x][];
		for (int y = 0; y < size.y; y++)
		{
			cells[y] = new WFCCell[(int)size.y]; ;
			for (int x = 0; x < size.x; x++)
			{
				message += var[x * size.AsVector2Int().x + y];
				if (y + 1 < size.y)
				{
					message += ", ";
				}
				else
				{
					message += "\n";
				}
			}
		}
		Debug.Log(message);

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
				WFCTile collapsedTile = tileDict[var[x * size.AsVector2Int().x + y]];

				SpriteRenderer sR = target.GetComponent<SpriteRenderer>();
				sR.sprite = collapsedTile.TileData.Sprite;
			}
		}
	}

	public GameObject[][] Export(WFCCell[][] input)
	{
		WFCCell[][] toExport;
		toExport = input;
		bool createGameObjects = Exported.Length != input.Length || Exported[0].Length != input[0].Length || Exported[0][0] == null;

		if (createGameObjects)
		{
			Exported = new GameObject[toExport.Length][];
		}

		for (int x = 0; x < toExport.Length; x++)
		{
			if (createGameObjects)
			{
				Exported[x] = new GameObject[toExport[0].Length];
			}
			for (int y = 0; y < toExport[0].Length; y++)
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

				if (toExport[x][y].CollapsedTile != null)
				{
					SpriteRenderer sR = target.GetComponent<SpriteRenderer>();
					sR.sprite = toExport[x][y].CollapsedTile.TileData.Sprite;
				}

				WFCCellComponent cellVisualiser = target.GetComponent<WFCCellComponent>();
				cellVisualiser.CellCaptured = toExport[x][y].GetCellStruct();
				cellVisualiser.UpdateVisuals();
			}
		}

		return Exported;
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
