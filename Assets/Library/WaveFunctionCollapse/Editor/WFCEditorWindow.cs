using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class WFCEditorWindow : ExtendedEditorWindow
{
	[MenuItem("Folvos Library/WFC Editor Window")]
	public static void ShowWindow()
	{
		GetWindow<WFCEditorWindow>("WFC Editor Window");
	}

	GameObject mapParent;
	[SerializeField] IWFCImporter importer;
	[SerializeField] IWFCManager manager;
	[SerializeField] IWFCExporter exporter;


	bool hasInitialized = false;

	private void OnGUI()
	{
		GUILayout.Label("Text on a screen");
		mapParent = (GameObject)EditorGUILayout.ObjectField("Map Parent: ", mapParent, typeof(GameObject), true);
		importer = (IWFCImporter)EditorGUILayout.ObjectField("Importer: ", (Object)importer, typeof(IWFCImporter), true);
		manager = (IWFCManager)EditorGUILayout.ObjectField("Manager: ", (Object)manager, typeof(IWFCManager), true);
		exporter = (IWFCExporter)EditorGUILayout.ObjectField("Exporter: ", (Object)exporter, typeof(IWFCExporter), true);

		int stepCount = 1;
		if (manager != null)
		{
			//When we have initialized we don't want to force update
			manager.DrawSize(!hasInitialized);
			stepCount = EditorGUILayout.IntSlider(stepCount, 1, 50);
		}

		if (GUILayout.Button("Reset!"))
		{
			if (mapParent != null)
			{
				for (int i = 0; i < mapParent.transform.childCount; i++)
				{
					DestroyImmediate(mapParent.transform.GetChild(0).gameObject);
				}

				hasInitialized = false;

				if (manager != null)
				{
					manager.ClearQueue();
				}

				if (exporter != null)
				{
					exporter.Reset();
				}

				if (importer != null)
				{
					importer.Reset();
				}

			}
		}
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Generate!") && mapParent != null)
		{
			if (manager == null)
			{
				Debug.LogError("Tried to generate when no manager is selected");
			}
			else
			{
				Initialize();
				manager.Generate();
			}
		}

		if (GUILayout.Button("Generate Step!") && mapParent != null)
		{
			if (manager == null)
			{
				Debug.LogError("Tried to generate when no manager is selected");
			}
			else
			{

				Initialize();
				manager.GenerateStep(stepCount);
			}
		}

		GUILayout.EndHorizontal();
	}

	void Initialize()
	{
		if (!hasInitialized)
		{
			hasInitialized = true;
			Debug.Log("Generate map");

			manager.SetImporter(importer);
			manager.SetExporter(exporter);

			exporter.SetParent(mapParent.transform);

			manager.Initialize();

			manager.OnResult += OnGenerateResult;
		}
	}

	void OnGenerateResult()
	{
		Debug.Log("Reached On Generate Result");
		GameObject[][] map = (exporter as BeachWFCExporter).Export((manager as WFCManager_2D).GetCells());
		int rowNumber = 0;

		Debug.Log($"Map.Length: {map.Length}");//, Map[0].Length: {map[0].Length}");

		foreach (GameObject[] row in map)
		{
			Transform rowParent = new GameObject($"Row{{{rowNumber}}}").transform;
			rowParent.SetParent(mapParent.transform);
			foreach (GameObject cell in row)
			{
				cell.transform.SetParent(rowParent);
			}
			rowNumber++;
		}
	}
}
