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
	IWFCImporter importer;
	IWFCManager manager;
	IWFCExporter exporter;
	private void OnGUI()
	{
		GUILayout.Label("Text on a screen");
		mapParent = (GameObject)EditorGUILayout.ObjectField("Map Parent: ", mapParent, typeof(GameObject), true);
		importer = (IWFCImporter)EditorGUILayout.ObjectField("Importer: ", (Object)importer, typeof(IWFCImporter), false);
		manager = (IWFCManager)EditorGUILayout.ObjectField("Manager: ", (Object)manager, typeof(IWFCManager), false);
		exporter = (IWFCExporter)EditorGUILayout.ObjectField("Exporter: ", (Object)exporter, typeof(IWFCExporter), false);

		if (manager != null)
		{
			manager.DrawSize();
		}

		if (GUILayout.Button("Generate!"))
		{
			if (manager == null)
			{
				Debug.LogError("Tried to generate when no manager is selected");
			}
			else
			{
				Debug.Log("Generate map");

				importer.Import<string>("https://www.reddit.com/r/196/comments/10nfwvk/boy_likerule/");

				if (!manager.HasInitialized())
				{
					manager.Initialize();
				}
				manager.OnResult += OnGenerateResult;
				manager.Generate();


			}
		}
	}

	void OnGenerateResult()
	{
		GameObject[][] map = (exporter as BeachWFCExporter).Export((manager as WFCManager_2D).GetCells());
		int rowNumber = 0;
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
