using FolvosLibrary.WFC;
using UnityEngine;


[CreateAssetMenu(menuName = "Folvos/WFC/Exporter/BeachExporter"), System.Serializable]
public class BeachWFCExporter : IWFCExporter
{
	public override void Export()
	{
		Debug.LogError("Export should not be called, use Export(WFCCell[][]) instead");
	}

	Transform parent;
	GameObject[][] Exported = new GameObject[0][];

	public GameObject[][] Export(WFCCell[][] input)
	{
		WFCCell[][] toExport;
		toExport = input as WFCCell[][];
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
