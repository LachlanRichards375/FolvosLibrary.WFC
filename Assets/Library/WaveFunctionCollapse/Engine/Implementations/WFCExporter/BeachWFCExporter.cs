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
	public GameObject[][] Export(IWFCCell[][] input)
	{
		IWFCCell[][] toExport;
		toExport = input as IWFCCell[][];

		GameObject[][] Exported = new GameObject[toExport.Length][];

		for (int x = 0; x < toExport.Length; x++)
		{
			Exported[x] = new GameObject[toExport[0].Length];
			for (int y = 0; y < toExport[0].Length; y++)
			{
				Debug.Log($"Creating gameobject at ({x},{y})");
				//{toExport[x][y].CollapsedTile.Name} 
				Exported[x][y] = new GameObject($"{{{x},{y}}}", typeof(SpriteRenderer));
				Transform t = Exported[x][y].transform;
				t.position = new Vector3(x, y, 0f);

				SpriteRenderer sR = Exported[x][y].GetComponent<SpriteRenderer>();
				if (input[x][y].CollapsedTile != null)
				{
					sR.sprite = input[x][y].CollapsedTile.TileData.Sprite;
				}
			}
		}

		return Exported;
	}
}
