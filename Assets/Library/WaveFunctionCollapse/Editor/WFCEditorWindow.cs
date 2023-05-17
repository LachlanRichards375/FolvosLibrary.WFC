using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;

public class WFCEditorWindow : EditorWindow
{
	[MenuItem("Folvos Library/WFC Editor Window")]
	public static void ShowWindow()
	{
		GetWindow<WFCEditorWindow>("WFC Editor Window");
	}

	public string RuleStorage = "Library/WaveFunctionCollapse/WFCRules";
	GameObject mapParent;
	IWFCImporter importer;
	IWFCManager manager;
	IWFCExporter exporter;
	private void OnGUI()
	{
		GUILayout.Label("Text on a screen");
		RuleStorage = EditorGUILayout.TextField("Label", RuleStorage);
		mapParent = (GameObject)EditorGUILayout.ObjectField("Map Parent: ", mapParent, typeof(GameObject), true);
		importer = (IWFCImporter)EditorGUILayout.ObjectField("Importer", (Object)importer, typeof(IWFCImporter), false);
	}
}
