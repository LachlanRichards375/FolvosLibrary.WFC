using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

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

		if (manager != null)
		{
			manager.DrawSize();
		}

		// if (GUILayout.Button("Clear Manager Domain"))
		// {

		// }

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Reset Initialization!"))
		{
			hasInitialized = false;
		}

		if (GUILayout.Button("Clear Entropy Queue!"))
		{
			((WFCManager_2D)manager).ClearQueue();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Generate!"))
		{
			if (manager == null)
			{
				Debug.LogError("Tried to generate when no manager is selected");
			}
			else
			{
				hasInitialized = true;
				Debug.Log("Generate map");

				manager.SetImporter(importer);
				manager.SetExporter(exporter);

				manager.Initialize();

				manager.OnResult += OnGenerateResult;
				manager.Generate();
			}
		}

		if (GUILayout.Button("Generate Step!"))
		{
			if (manager == null)
			{
				Debug.LogError("Tried to generate when no manager is selected");
			}
			else
			{

				if (!hasInitialized)
				{
					hasInitialized = true;
					Debug.Log("Generate map");

					manager.SetImporter(importer);
					manager.SetExporter(exporter);

					manager.Initialize();

					manager.OnResult += OnGenerateResult;
				}
				manager.GenerateStep(2);
			}
		}

		GUILayout.EndHorizontal();
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
