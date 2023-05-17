using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FolvosLibrary.WFC
{
	// [CustomPropertyDrawer(typeof(WFCRule))]
	// public class WFCRuleDrawer : PropertyDrawer
	// {
	// 	bool dropDown;

	// 	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	// 	{
	// 		EditorGUI.BeginProperty(position, label, property);

	// 		// WFCRule test = property.objectReferenceValue as WFCRule;
	// 		WFCRule[] WFCRuleList = fieldInfo.GetValue(property.serializedObject.targetObject) as WFCRule[];
	// 		SerializedProperty rules = property.serializedObject.FindProperty("Rules");

	// 		int index;
	// 		for (index = 0; index < rules.arraySize; index++)
	// 		{
	// 			if (rules.GetArrayElementAtIndex(index).displayName == property.displayName)
	// 			{
	// 				break;
	// 			}
	// 		}

	// 		string selectedType = "";
	// 		if (WFCRuleList[index] == null)
	// 		{
	// 			selectedType = "Null";
	// 		}
	// 		else
	// 		{
	// 			selectedType = WFCRuleList[index].GetType().ToString();
	// 		}

	// 		if (EditorGUILayout.DropdownButton(new GUIContent(selectedType), FocusType.Passive))
	// 		{
	// 			GenericMenu menu = new GenericMenu();

	// 			var subclassTypes = Assembly
	// 				.GetAssembly(typeof(WFCRule))
	// 				.GetTypes()
	// 				.Where(t => t.IsSubclassOf(typeof(WFCRule)));

	// 			foreach (System.Type rule in subclassTypes)
	// 			{
	// 				if (rule.IsSubclassOf(typeof(WFCRule)) && !rule.IsAbstract)
	// 				{
	// 					AddWFCRule(menu, rule, property);
	// 				}
	// 			}

	// 			menu.ShowAsContext();
	// 		}

	// 		EditorGUILayout.PropertyField(property);

	// 		EditorGUI.EndProperty();
	// 	}

	// 	void AddWFCRule(GenericMenu menu, System.Type type, SerializedProperty property)
	// 	{
	// 		GUIContent label = new GUIContent(type.FullName);

	// 		menu.AddItem(label, true, () =>
	// 		{
	// 			Debug.Log("Test");
	// 			WFCRule rule = (WFCRule)ScriptableObject.CreateInstance(type);
	// 			SerializedProperty prop = property;
	// 			MenuPressed(rule, prop);
	// 		});
	// 	}

	// 	void MenuPressed(WFCRule createdRule, SerializedProperty property)
	// 	{
	// 		Debug.Log(property.propertyType);
	// 		Debug.Log("Pressed property, created rule: " + createdRule.GetType() + " from property: " + property.name);
	// 		property.objectReferenceValue = createdRule;
	// 		property.serializedObject.ApplyModifiedProperties();
	// 	}
	// }

}