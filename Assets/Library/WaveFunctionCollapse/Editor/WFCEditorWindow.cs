using System;
using FolvosLibrary.Logging;
using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class WFCEditorWindow : ExtendedEditorWindow
{
	[MenuItem("Folvos Library/WFC Editor Window")]
	public static void ShowWindow()
	{
		GetWindow<WFCEditorWindow>("WFC DLL Editor Window");
	}

	[SerializeReference] GameObject mapParent;
	[SerializeReference] WFCTileList wfcTileList;
	[SerializeField] IWFCExporter exporter;
	[SerializeField] Vector2Int gridSize;

	private void OnGUI()
	{
		ShowDLLOptions();
		ShowDllButtons();
	}

	void ShowDLLOptions()
	{
		mapParent = (GameObject)EditorGUILayout.ObjectField("Map Parent: ", mapParent, typeof(GameObject), true);
		wfcTileList = (WFCTileList)EditorGUILayout.ObjectField("Tile List: ", wfcTileList, typeof(WFCTileList), true);
		exporter = (IWFCExporter)EditorGUILayout.ObjectField("Exporter: ", exporter, typeof(IWFCExporter), true);
		gridSize = (Vector2Int)EditorGUILayout.Vector2IntField("Size: ", gridSize);
	}

	[SerializeField] WaveFunctionCollapse_CPP dll;

	void ShowDllButtons()
	{
		dll ??= new WaveFunctionCollapse_CPP();

		DrawLine(20, 20);

		if (GUILayout.Button("Reset DLL"))
		{
			Reset();
		}

		if (GUILayout.Button("Import to DLL"))
		{
			Import();
		}

		if (GUILayout.Button("Initialize and set Size to (5,5)") && dll != null)
		{
			Initialize();
		}

		if (GUILayout.Button("collapse specific cell to sand DLL") && dll != null)
		{
			CollapseSpecificCell();
		}

		if (GUILayout.Button("Run DLL") && dll != null)
		{
			Run();
		}

		if (GUILayout.Button("Export DLL Results") && dll != null)
		{
			Export();
		}
	}

	private void Reset()
	{
		dll = new WaveFunctionCollapse_CPP();
		while (mapParent.transform.childCount > 0)
		{
			DestroyImmediate(mapParent.transform.GetChild(0).gameObject);
		}
	}

	void Import()
	{
		dll = new WaveFunctionCollapse_CPP();
		WFCTile[] toAdd = wfcTileList.tiles;
		string message = "Added: ";
		foreach (WFCTile tile in toAdd)
		{
			message += tile.ID + " as " + tile.name + "; ";
			dll.AddTilesToDomain(tile.ID);
			foreach (WFCRule rule in tile.Rules)
			{
				if (rule is MultiCellIsNotTarget2D)
				{
					MultiCellIsNotTarget2D multiTargetRule = rule as MultiCellIsNotTarget2D;
					WaveFunctionCollapse_CPP.CellIsNotRule ruleToAdd = new WaveFunctionCollapse_CPP.CellIsNotRule();
					ruleToAdd.tile = tile.ID;
					ruleToAdd.goal = multiTargetRule.goal.ID;
					ruleToAdd.localTargets = multiTargetRule.GetTargetCellsArray();
					dll.AddCellIsNotRule(ruleToAdd);
				}
			}
		}

		Debug.Log("Imported successfully\n" + message);
	}

	void Initialize()
	{
		dll.Create2DWFC(new WFCPosition(gridSize));
		Debug.Log("Initialized successfully");
	}

	void CollapseSpecificCell()
	{
		dll.CollapseSpecificCell(4, new WFCPosition((int)gridSize.x / 2, (int)gridSize.y / 2));
		Debug.Log("Collapsed Specific Cell successfully");
	}

	void Run()
	{
		System.Diagnostics.Stopwatch stopwatch = new();
		stopwatch.Start();
		dll.RunGenerator();
		stopwatch.Stop();
		Debug.Log("Run the Generator in " + stopwatch.ElapsedMilliseconds + "ms successfully");
	}

	void Export()
	{
		ulong[] test = dll.GetResults();
		Debug.Log("Exported from the Generator successfully, recieved " + test.Length + "results");
		exporter.SetParent(mapParent.transform);
		exporter.Export(test, new WFCPosition(gridSize), wfcTileList);
	}

	string TimeToGenerate(DateTime start, DateTime end)
	{
		TimeSpan finalTime = end.Subtract(start);
		string returner = "";
		if (finalTime.Minutes > 0)
		{
			returner += finalTime.Minutes + "m:";
		}
		//Always include seconds if minutes is > 0
		if (finalTime.Minutes > 0 || finalTime.Seconds > 0)
		{
			returner += finalTime.Seconds + "s.";
		}

		return returner + finalTime.Milliseconds + "ms";
	}
}
