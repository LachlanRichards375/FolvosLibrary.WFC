using FolvosLibrary.WFC;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class WFCTileEditorWindow : ExtendedEditorWindow
{
	public static void Open(WFCTile wfcTile)
	{
		WFCTileEditorWindow window = GetWindow<WFCTileEditorWindow>();
		window.serializedObject = new SerializedObject(wfcTile);
	}

	private void OnGUI()
	{
		if (serializedObject == null)
		{
			EditorGUILayout.LabelField("Select a tile to edit");
			return;
		}
		serializedObject.Update();
		DrawField("Name", false);
		DrawField("TileWeight", false);
		DrawField("TileData", false);
		DrawLine();

		DrawRules();

		Apply();
		serializedObject.Update();
	}

	void DrawRules()
	{
		GUILayout.BeginHorizontal();

		//~~~~~~~~~~~~~~~\
		// DrawField("Rules.Array.data[0]", false);
		//~~~~~~~~~~~~~~~\\
		GUILayout.BeginVertical("box", GUILayout.MaxWidth(100), GUILayout.ExpandHeight(true));
		GUILayout.Label("Rules: ");
		DrawSidebar(serializedObject.FindProperty("Rules"));

		GUILayout.FlexibleSpace();
		SerializedProperty rules = serializedObject.FindProperty("Rules");
		if (GUILayout.Button("Add Rule"))
		{
			rules.InsertArrayElementAtIndex(rules.arraySize);
		}
		if (selectedProperty != null)
		{
			if (GUILayout.Button("Remove Selected Rule"))
			{
				int index;
				for (index = 0; index < rules.arraySize; index++)
				{
					if (rules.GetArrayElementAtIndex(index).displayName == selectedProperty.displayName)
					{
						break;
					}
				}
				rules.DeleteArrayElementAtIndex(index);
				selectedProperty = null;
			}
		}

		GUILayout.EndVertical();
		GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
		if (selectedProperty != null)
		{
			NewRuleEdit();
		}
		else
		{
			EditorGUILayout.LabelField("Select a rule from the list");
		}
		GUILayout.EndVertical();
		//~~~~~~~~~~~~~~~~~~~\\
		GUILayout.EndHorizontal();
	}

	void NewRuleEdit()
	{
		EditorGUILayout.LabelField("Selected: " + selectedProperty.displayName);

		WFCTile tile = (selectedProperty.serializedObject.targetObject as WFCTile);

		int arrayIndex = int.Parse(selectedProperty.displayName.Split(" ").Last());

		WFCRule currentRule = tile.Rules[arrayIndex];

		DrawRuleChangeDropDown(tile, arrayIndex);

		currentRule?.DrawRuleProperties();
	}

	void DrawRuleChangeDropDown(WFCTile currentTile, int ruleIndex)
	{
		WFCRule currentRule = currentTile.Rules[ruleIndex];
		string selectedType = "";
		if (currentRule == null)
		{
			selectedType = "Null";
		}
		else
		{
			selectedType = currentRule.GetType().ToString();
		}

		if (EditorGUILayout.DropdownButton(new GUIContent(selectedType), FocusType.Passive))
		{
			OnChangeRule(currentTile, ruleIndex);
		}
	}

	void OnChangeRule(WFCTile currentTile, int ruleIndex)
	{
		GenericMenu menu = new GenericMenu();

		var subclassTypes = Assembly
			.GetAssembly(typeof(WFCRule))
			.GetTypes()
			.Where(t => t.IsSubclassOf(typeof(WFCRule)));

		foreach (System.Type rule in subclassTypes)
		{
			if (rule.IsSubclassOf(typeof(WFCRule)) && !rule.IsAbstract)
			{
				GUIContent label = new GUIContent(rule.FullName);
				System.Type currentRuleType = currentTile.Rules[ruleIndex]?.GetType();

				menu.AddItem(label, rule == currentRuleType, () =>
				{
					//Apply changes
					selectedProperty.serializedObject.ApplyModifiedProperties();
					//Change the specific rule
					currentTile.Rules[ruleIndex] = (WFCRule)System.Activator.CreateInstance(rule);
					//Update the tile to reflect changes
					selectedProperty.serializedObject.Update();
				});
			}
		}

		menu.ShowAsContext();
	}
}
