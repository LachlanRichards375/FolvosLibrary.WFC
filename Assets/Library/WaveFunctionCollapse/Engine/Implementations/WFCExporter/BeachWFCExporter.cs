using System.Collections;
using System.Collections.Generic;
using System.Xml;
using FolvosLibrary.WFC;
using UnityEngine;


[CreateAssetMenu(menuName = "WFC/Exporter/BeachExporter")]
public class BeachWFCExporter : ScriptableObject, IWFCExporter
{
	public void Export()
	{
		Debug.LogError("Export should not be called, use Export(IWFCCell[][]) instead");
	}

	GameObject[][] Exported = new GameObject[0][];

	public GameObject[][] Export(IWFCCell[][] input)
	{
		IWFCCell[][] toExport;
		toExport = input as IWFCCell[][];
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
				GameObject target;
				if (createGameObjects)
				{
					Exported[x][y] = new GameObject($"{{{x},{y}}}", typeof(SpriteRenderer));
				}
				target = Exported[x][y];
				Transform t = target.transform;
				t.position = new Vector3(x, y, 0f);

				SpriteRenderer sR = target.GetComponent<SpriteRenderer>();
				if (input[x][y].CollapsedTile != null)
				{
					sR.sprite = input[x][y].CollapsedTile.TileData.Sprite;
				}
			}
		}

		return Exported;
	}
}
