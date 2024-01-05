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
	[SerializeField] IWFCGrid grid;
	[SerializeField] IWFCCollapseMethod collapseMethod;

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

		ShowDllOptions();
	}

	[SerializeField] WaveFunctionCollapse_CPP dll;

	void ShowDllOptions()
	{
		dll ??= new WaveFunctionCollapse_CPP();
		DrawLine(20, 20);

		if (GUILayout.Button("Import to DLL"))
		{
			dll = new WaveFunctionCollapse_CPP();
			WFCTile[] toAdd = importer.Import<string>("Needed to provide some type");
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


		if (GUILayout.Button("Initialize and set Size to (5,5)") && dll != null)
		{
			dll.Create2DWFC(new WFCPosition(5, 5));
			Debug.Log("Initialized successfully");
		}

		if (GUILayout.Button("collapse specific cell to sand DLL") && dll != null)
		{
			dll.CollapseSpecificCell(4, new WFCPosition(2, 2));
			Debug.Log("Collapsed Specific Cell successfully");
		}

		if (GUILayout.Button("Run DLL") && dll != null)
		{
			System.Diagnostics.Stopwatch stopwatch = new();
			stopwatch.Start();
			dll.RunGenerator();
			stopwatch.Stop();
			Debug.Log("Run the Generator in " + stopwatch.ElapsedMilliseconds + "ms successfully");
		}

		if (GUILayout.Button("Export DLL Results") && dll != null)
		{
			ulong[] test = dll.GetResults();
			Debug.Log("Exported from the Generator successfully, recieved " + test.Length + "results");

			exporter.Export(test, new WFCPosition(5, 5));
		}

	}


	int stepCount = 1;
	int millsBetweenStep = 500;
	void DisplayVariableSetters()
	{
		DrawObjectPickers();
		ConfigureObjects();

		DrawLine(20);

		DrawObjectSettings();

		DrawLine(20);
	}

	void DrawObjectPickers()
	{
		mapParent = (GameObject)EditorGUILayout.ObjectField("Map Parent: ", mapParent, typeof(GameObject), true);
		importer = (IWFCImporter)EditorGUILayout.ObjectField("Importer: ", (UnityEngine.Object)importer, typeof(IWFCImporter), true);
		manager = (IWFCManager)EditorGUILayout.ObjectField("Manager: ", (UnityEngine.Object)manager, typeof(IWFCManager), true);
		exporter = (IWFCExporter)EditorGUILayout.ObjectField("Exporter: ", (UnityEngine.Object)exporter, typeof(IWFCExporter), true);
		grid = (IWFCGrid)EditorGUILayout.ObjectField("Grid: ", (UnityEngine.Object)grid, typeof(IWFCGrid), true);
		collapseMethod = (IWFCCollapseMethod)EditorGUILayout.ObjectField("Collapse Method: ", (UnityEngine.Object)collapseMethod, typeof(IWFCCollapseMethod), true);
	}

	void ConfigureObjects()
	{
		if (manager != null)
		{
			if (grid != null)
			{
				grid.SetManager(manager);
				manager.SetGrid(grid);
			}

			if (collapseMethod != null)
			{
				manager.SetCollapseMethod(collapseMethod);
				collapseMethod.SetManager(manager);
			}
		}
	}

	void DrawObjectSettings()
	{
		Logging.LoggingLevel = (Logging.Priority)EditorGUILayout.EnumPopup("Logging Level", Logging.LoggingLevel);
		Logging.LoggingGroups = (Logging.ProjectGroups)EditorGUILayout.EnumFlagsField("Messages to display", Logging.LoggingGroups);

		if (manager != null)
		{
			stepCount = EditorGUILayout.IntSlider(stepCount, 1, 50);
			manager.MaxThreadCount = EditorGUILayout.IntField("Simultaneous Thread Count", manager.MaxThreadCount);
		}
		else
		{
			EditorGUILayout.LabelField("Manager has not been provided", GUILayout.MinHeight(30));
		}

		if (grid != null)
		{
			//When we have initialized we don't want to force update
			grid.DrawSize(!hasInitialized);
		}
		else
		{
			EditorGUILayout.LabelField("Grid has not been provided", GUILayout.MinHeight(30));
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
					manager.Reset();
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
				DateTime initializationStartTime = DateTime.Now;
				Initialize();
				Debug.Log($"Time to Initialize: {TimeToGenerate(initializationStartTime, DateTime.Now)}");
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
			SubscribeToResults();
		}
	}

	void SubscribeToResults()
	{
		manager.OnResult += OnGenerateResult;
		manager.OnError += OnGenerateError;
	}

	void UnsubscribeToResults()
	{
		manager.OnResult -= OnGenerateResult;
		manager.OnError -= OnGenerateError;
	}

	void OnGenerateResult()
	{
		UnsubscribeToResults();
		ResetTimelapseVariables();
		Debug.Log($"Time to Generate: {TimeToGenerate(startTime, DateTime.Now)}");
		Debug.Log("Reached On Generate Result");

		manager.UpdateOutput();
	}

	void OnGenerateError(Exception exception)
	{
		UnsubscribeToResults();
		ResetTimelapseVariables();
		Debug.Log($"Time to Error: {TimeToGenerate(startTime, DateTime.Now)}");
		Debug.Log(exception.Message);

		manager.UpdateOutput();
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
