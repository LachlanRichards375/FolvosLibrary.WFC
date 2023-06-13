using System;
using System.Threading;
using System.Threading.Tasks;
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
		GetWindow<WFCEditorWindow>("WFC Editor Window");
	}

	GameObject mapParent;
	[SerializeField] IWFCImporter importer;
	[SerializeField] IWFCManager manager;
	[SerializeField] IWFCExporter exporter;

	bool hasInitialized = false;
	DateTime startTime;

	private void OnGUI()
	{
		DisplayVariableSetters();
		DisplayResetButton();

		//enable if we're not doing a timelapse AND MapParent is not null
		GUI.enabled = (GenerateTimeLapseTask == null && mapParent != null);
		DisplayGenerateButtons();
		GUI.enabled = true;
		if (GenerateTimeLapseTask != null)
		{
			DisplayTimelapseControls();
		}
	}

	int stepCount = 1;
	int millsBetweenStep = 500;
	void DisplayVariableSetters()
	{
		mapParent = (GameObject)EditorGUILayout.ObjectField("Map Parent: ", mapParent, typeof(GameObject), true);
		importer = (IWFCImporter)EditorGUILayout.ObjectField("Importer: ", (UnityEngine.Object)importer, typeof(IWFCImporter), true);
		manager = (IWFCManager)EditorGUILayout.ObjectField("Manager: ", (UnityEngine.Object)manager, typeof(IWFCManager), true);
		exporter = (IWFCExporter)EditorGUILayout.ObjectField("Exporter: ", (UnityEngine.Object)exporter, typeof(IWFCExporter), true);

		Logging.LoggingLevel = (Logging.Priority)EditorGUILayout.EnumPopup("Logging Level", Logging.LoggingLevel);
		Logging.LoggingGroups = (Logging.ProjectGroups)EditorGUILayout.EnumFlagsField("Messages to display", Logging.LoggingGroups);

		if (manager != null)
		{
			//When we have initialized we don't want to force update
			manager.DrawSize(!hasInitialized);
			stepCount = EditorGUILayout.IntSlider(stepCount, 1, 50);
		}

		millsBetweenStep = EditorGUILayout.IntField("Milliseconds between step", millsBetweenStep);
		if (millsBetweenStep <= 0)
		{
			millsBetweenStep = 1;
		}
	}

	void DisplayResetButton()
	{
		if (GUILayout.Button("Reset!"))
		{
			if (mapParent != null)
			{
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

				while (mapParent.transform.childCount > 0)
				{
					DestroyImmediate(mapParent.transform.GetChild(0).gameObject);
				}
			}

			ResetTimelapseVariables();
		}
	}

	void DisplayGenerateButtons()
	{
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
				startTime = DateTime.Now;
				Debug.Log($"Time at generation start: {startTime.TimeOfDay}");
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

		if (GUILayout.Button("Generate Timelapse") && mapParent != null)
		{
			if (manager == null)
			{
				Debug.LogError("Tried to generate when no manager is selected");
			}
			else
			{

				Initialize();
				GenerateTimeLapseTask = manager.GenerateTimeLapse(CancelTimeLapseToken, millsBetweenStep);
			}
		}

		GUILayout.EndHorizontal();
	}

	Task GenerateTimeLapseTask;
	CancellationTokenSource CancelTimeLapseToken = new CancellationTokenSource();
	void DisplayTimelapseControls()
	{
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Stop Generation"))
		{
			CancelTimeLapseToken.Cancel();
			ResetTimelapseVariables();
		}

		GUILayout.EndHorizontal();
	}

	void ResetTimelapseVariables()
	{
		GenerateTimeLapseTask = null;
		CancelTimeLapseToken = new CancellationTokenSource();
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
		manager.OnResult -= OnGenerateResult;
		ResetTimelapseVariables();
		Debug.Log($"Time to Generate: {TimeToGenerate()}");
		Debug.Log("Reached On Generate Result");

		GameObject[][] map = (exporter as BeachWFCExporter).Export((manager as WFCManager_2D).GetCells());
		int rowNumber = 0;

		Debug.Log($"Map.Length: {map.Length}");

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
	string TimeToGenerate()
	{
		TimeSpan finalTime = DateTime.Now.Subtract(startTime);
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
