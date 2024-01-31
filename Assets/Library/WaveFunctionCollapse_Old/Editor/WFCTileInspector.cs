using FolvosLibrary.WFC;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AssetHandler
{
	[OnOpenAsset()]
	public static bool OpenEditor(int instanceId, int line)
	{
		WFCTile obj = EditorUtility.InstanceIDToObject(instanceId) as WFCTile;
		if (obj != null)
		{
			WFCTileEditorWindow.Open(obj);
			return true;
		}
		return false;
	}
}

[CustomEditor(typeof(WFCTile))]
public class WFCTileInspector : Editor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Open Editor"))
		{
			WFCTileEditorWindow.Open((WFCTile)target);
		}
		base.DrawDefaultInspector();
	}
}
