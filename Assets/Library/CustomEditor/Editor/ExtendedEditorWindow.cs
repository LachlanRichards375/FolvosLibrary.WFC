using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ExtendedEditorWindow : EditorWindow
{
	[SerializeField] protected SerializedObject serializedObject;
	[SerializeField] protected SerializedProperty currentProperty;

	private string selectedPropertyPath;
	protected SerializedProperty selectedProperty;

	protected void DrawProperties(SerializedProperty prop, bool drawChildren)
	{
		DrawField(prop.propertyPath, false);
		string lastPropPath = string.Empty;
		foreach (SerializedProperty p in prop)
		{
			// DrawField(p.propertyPath, false);

			//Draw Arrays
			if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
			{
				EditorGUILayout.BeginHorizontal();
				p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
				EditorGUILayout.EndHorizontal();

				if (p.isExpanded)
				{
					EditorGUI.indentLevel++;
					DrawProperties(p, drawChildren);
					EditorGUI.indentLevel--;
				}
			}
			//Draw non-arrays
			else
			{
				if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
				lastPropPath = p.propertyPath;
				EditorGUILayout.PropertyField(p, drawChildren);
			}
		}
	}

	protected void DrawSidebar(SerializedProperty prop)
	{
		foreach (SerializedProperty p in prop)
		{
			if (GUILayout.Button(p.displayName))
			{
				selectedPropertyPath = p.propertyPath;
				selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
			}
		}

		if (!string.IsNullOrEmpty(selectedPropertyPath))
		{
			selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
		}
	}

	protected void DrawField(string propName, bool relative)
	{
		if (relative && currentProperty != null)
		{
			EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);
		}
		else if (serializedObject != null)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
		}
	}

	protected void DrawLine(int space = 5)
	{
		EditorGUILayout.Space(space);
		var rect = EditorGUILayout.BeginHorizontal();
		Handles.color = new Color(0, 0, 0, 0.3f);
		Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space(space);
	}

	protected void Apply()
	{
		serializedObject.ApplyModifiedProperties();
	}
}
