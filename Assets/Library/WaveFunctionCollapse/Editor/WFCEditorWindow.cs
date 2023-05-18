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

		if(manager!= null){
			manager.DrawSize();
		}

		if (GUILayout.Button("Generate!"))
		{
			if(manager == null){
				Debug.LogError("Tried to generate when no manager is selected");
			} else {
				Debug.Log("Generate map");

				if(!manager.HasInitialized()){
					manager.Initialize();
				}
				manager.Generate();
			}
		}
	}
}
