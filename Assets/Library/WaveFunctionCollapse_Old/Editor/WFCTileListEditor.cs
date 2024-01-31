
using FolvosLibrary.WFC;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(WFCTileList))]
public class WFCTileListDrawer : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		WFCTileList tileList = (WFCTileList)target;
		if (GUILayout.Button("Validate Bitmask"))
		{
			bool valid = tileList.ValidateTileBitmasks();
		}
	}
}